﻿using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.IO;
using Svg;
using System.Drawing.Imaging;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Text;

namespace highchart_export_module_asp_net
{
    public partial class highcharts_export : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                if (Request.Form["type"] != null && Request.Form["svg"] != null && Request.Form["filename"] != null)
                {
                    string tType = Request.Form["type"].ToString();
                    string tSvg = Request.Form["svg"].ToString();
                    string tFileName = Request.Form["filename"].ToString();
                    if (tFileName == "")
                    {
                        tFileName = "chart";
                    }

                    MemoryStream tData = new MemoryStream(Encoding.ASCII.GetBytes(tSvg));
                    MemoryStream tStream = new MemoryStream();

                    string tTmp = new Random().Next().ToString();

                    string tExt = "";
                    string tTypeString = "";

                    switch (tType)
                    {
                        case "image/png":
                            tTypeString = "-m image/png";
                            tExt = "png";
                            break;
                        case "image/jpeg":
                            tTypeString = "-m image/jpeg";
                            tExt = "jpg";
                            break;
                        case "application/pdf":
                            tTypeString = "-m application/pdf";
                            tExt = "pdf";
                            break;
                        case "image/svg+xml":
                            tTypeString = "-m image/svg+xml";
                            tExt = "svg";
                            break;
                    }

                    if (tTypeString != "")
                    {
                        string tWidth = Request.Form["width"].ToString();
                        Svg.SvgDocument tSvgObj = SvgDocument.Open(tData);
                
                        switch (tExt)
                        {
                            case "jpg":
                                tSvgObj.Draw().Save(tStream, ImageFormat.Jpeg);
                                //DELETING SVG FILE 
                                //if (File.Exists(Server.MapPath(null) + ConfigurationManager.AppSettings["EXPORT_TEMP_FOLDER"].ToString() + tSvgOuputFile))
                                //{
                                //    File.Delete(Server.MapPath(null) + ConfigurationManager.AppSettings["EXPORT_TEMP_FOLDER"].ToString() + tSvgOuputFile);
                                //}
                                break;
                            case "png":
                                tSvgObj.Draw().Save(tStream, ImageFormat.Png);
                                ////DELETING SVG FILE
                                //if (File.Exists(Server.MapPath(null) + ConfigurationManager.AppSettings["EXPORT_TEMP_FOLDER"].ToString() + tSvgOuputFile))
                                //{
                                //    File.Delete(Server.MapPath(null) + ConfigurationManager.AppSettings["EXPORT_TEMP_FOLDER"].ToString() + tSvgOuputFile);
                                //}
                                break;
                            case "pdf":

                                PdfWriter tWriter = null;
                                Document tDocumentPdf = null;
                                try
                                {
                                    // First step saving png that would be used in pdf
                                    tSvgObj.Draw().Save(tStream, ImageFormat.Png);

                                    // Creating pdf document
                                    tDocumentPdf = new Document(new iTextSharp.text.Rectangle((float)tSvgObj.Width, (float)tSvgObj.Height));
                                    // setting up margin to full screen image
                                    tDocumentPdf.SetMargins(0.0f, 0.0f, 0.0f, 0.0f);
                                    // creating image
                                    iTextSharp.text.Image tGraph = iTextSharp.text.Image.GetInstance(tStream.ToArray());
                                    tGraph.ScaleToFit((float)tSvgObj.Width, (float)tSvgObj.Height);

                                    tStream = new MemoryStream();

                                    // Insert content
                                    tWriter = PdfWriter.GetInstance(tDocumentPdf,tStream);
                                    tDocumentPdf.Open();
                                    tDocumentPdf.NewPage();
                                    tDocumentPdf.Add(tGraph);
                                    tDocumentPdf.CloseDocument();

                                    //// deleting png
                                    //if (File.Exists(Server.MapPath(null) + ConfigurationManager.AppSettings["EXPORT_TEMP_FOLDER"].ToString() + tOutputFile.Substring(0, tOutputFile.LastIndexOf(".")) + ".png"))
                                    //{
                                    //    File.Delete(Server.MapPath(null) + ConfigurationManager.AppSettings["EXPORT_TEMP_FOLDER"].ToString() + tOutputFile.Substring(0, tOutputFile.LastIndexOf(".")) + ".png");
                                    //}

                                    //// deleting svg
                                    //if (File.Exists(Server.MapPath(null) + ConfigurationManager.AppSettings["EXPORT_TEMP_FOLDER"].ToString() + tSvgOuputFile))
                                    //{
                                    //    File.Delete(Server.MapPath(null) + ConfigurationManager.AppSettings["EXPORT_TEMP_FOLDER"].ToString() + tSvgOuputFile);
                                    //}

                                }
                                catch (Exception ex)
                                {
                                    throw ex;

                                }
                                finally
                                {
                                    tDocumentPdf.Close();
                                    tDocumentPdf.Dispose();
                                    tWriter.Close();
                                    tWriter.Dispose();
                                    tData.Dispose();
                                    tData.Close();

                                }


                                break;

                            case "svg":
                                tStream = tData;
                                break;
                        }





                        //if (File.Exists(Server.MapPath(null) + ConfigurationManager.AppSettings["EXPORT_TEMP_FOLDER"].ToString() + tOutputFile))
                        //{

                            //Putting session to be able to delete file in temp directory
                            //Session["sFileToTransmit_highcharts_export"] = File.ReadAllBytes(Server.MapPath(null) + ConfigurationManager.AppSettings["EXPORT_TEMP_FOLDER"].ToString() + tOutputFile);
                            //First step deleting disk file;
                            //File.Delete(Server.MapPath(null) + ConfigurationManager.AppSettings["EXPORT_TEMP_FOLDER"].ToString() + tOutputFile);
                            
                            Response.ClearContent();
                            Response.ClearHeaders();
                            Response.ContentType = tType;
                            Response.AppendHeader("Content-Disposition", "attachment; filename=" + tFileName + "." + tExt + "");
                            Response.BinaryWrite(tStream.ToArray());
                            Response.End();
                        //}

                    }
                }
            }
        }
    }
}
