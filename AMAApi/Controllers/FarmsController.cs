using FarmsApi.DataModels;
//using iTextSharp.text.pdf;
//using iTextSharp.text;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Web.Http;
using Spire.Pdf;
using System.Drawing.Imaging;
using System;
using System.Drawing;
using Spire.Pdf.Graphics;
using System.Linq;
using FarmsApi.DataModels;
using System.Web;
using iTextSharp.text.xml.xmp;

namespace FarmsApi.Services
{
    [RoutePrefix("farms")]
    public class FarmsController : ApiController
    {



        //[Authorize] http://localhost:44033/farms/GetHtml
        [Route("GetHtml")]
        [HttpGet]
        public IHttpActionResult GetHtml()
        {
            PdfDocument doc = new PdfDocument();

            // Load a PDF document
            doc.LoadFromFile(@"C:\\Dev\Shmia\AMAApi\bin\Sample.pdf");


            //PdfConvertOptions pdfToHtmlOptions = doc.ConvertOptions;

            //pdfToHtmlOptions.SetPdfToHtmlOptions(false, true, 1, false);

            doc.SaveToFile(@"C:\\Dev\Shmia\AMAApi\bin\PdfToHtmlWithCustomOptions.html", FileFormat.HTML);

            doc.Close();


            return Ok("true");

        }

        //[Authorize] http://localhost:44033/farms/GetImage
        [Route("GetImage")]
        [HttpGet]
        public IHttpActionResult GetImage()
        {

            // Create an instance of the PdfDocument class

            PdfDocument doc = new PdfDocument();

            // Load a PDF document
            doc.LoadFromFile(@"C:\\Dev\Shmia\AMAApi\bin\Sample.pdf");




            // Set the conversion options to embed images in the resulting HTML and limit one page per HTML file


            //Convert PDF pages to images
            Image[] images = SaveAsImage(doc);

            //Combine the images and save them as a multi-page TIFF file
            JoinTiffImages(images, @"C:\\Dev\Shmia\AMAApi\bin\result.tiff", EncoderValue.CompressionLZW);


            // // Save the PDF document to HTML format

            //doc.SaveToFile(@"C:\\Dev\Shmia\AMAApi\bin\PdfToHtmlWithCustomOptions.html", FileFormat.HTML);

            //doc.Close();


            return Ok("true");
        }

        private static Image[] SaveAsImage(PdfDocument document)
        {
            //Create a new image array
            Image[] images = new Image[document.Pages.Count];

            //Iterate through all pages in the document
            for (int i = 0; i < document.Pages.Count; i++)
            {
                //Convert a specific page to an image
                images[i] = document.SaveAsImage(i);
            }
            return images;
        }

        private static ImageCodecInfo GetEncoderInfo(string mimeType)
        {
            //Get the image encoders
            ImageCodecInfo[] encoders = ImageCodecInfo.GetImageEncoders();
            for (int j = 0; j < encoders.Length; j++)
            {
                //Find the encoder that matches the specified MIME type
                if (encoders[j].MimeType == mimeType)
                    return encoders[j];
            }
            throw new Exception(mimeType + " mime type not found in ImageCodecInfo");
        }

        public static void JoinTiffImages(Image[] images, string outFile, EncoderValue compressEncoder)
        {
            //Set the encoder parameters 
            Encoder enc = Encoder.SaveFlag;
            EncoderParameters ep = new EncoderParameters(2);
            ep.Param[0] = new EncoderParameter(enc, (long)EncoderValue.MultiFrame);
            ep.Param[1] = new EncoderParameter(Encoder.Compression, (long)compressEncoder);

            //Get the first image
            Image pages = images[0];
            //Initialize a frame
            int frame = 0;
            //Get an ImageCodecInfo object for processing TIFF image codec information
            ImageCodecInfo info = GetEncoderInfo("image/tiff");

            //Iterate through each Image
            foreach (Image img in images)
            {
                //If it's the first frame, save it to the output file with specified encoder parameters
                if (frame == 0)
                {
                    pages = img;
                    pages.Save(outFile, info, ep);
                }

                else
                {
                    //Save the intermediate frames
                    ep.Param[0] = new EncoderParameter(enc, (long)EncoderValue.FrameDimensionPage);

                    pages.SaveAdd(img, ep);
                }
                //If it's the last frame, flush the encoder parameters and close the file
                if (frame == images.Length - 1)
                {
                    ep.Param[0] = new EncoderParameter(enc, (long)EncoderValue.Flush);
                    pages.SaveAdd(ep);
                }
                frame++;
            }
        }

        // http://localhost:44033/farms/GetPdf
        [Route("GetPdf")]
        [HttpGet]
        public IHttpActionResult GetPdf()
        {
            //Create a PdfDocument object
            PdfDocument pdf = new PdfDocument();

            //Load a TIFF image
            Image tiffImage = Image.FromFile(@"C:\\Dev\Shmia\AMAApi\bin\result.tiff");

            //Split the Tiff image into separate images
            Image[] images = SplitTiffImage(tiffImage);

            //Iterate through the images
            for (int i = 0; i < images.Length; i++)
            {
                //Convert a specified image into a PDF image
                PdfImage pdfImg = PdfImage.FromImage(images[i]);

                //Get image width and height
                float width = pdfImg.Width;
                float height = pdfImg.Height;

                //Add a page with the same size as the image
                SizeF size = new SizeF(width, height);
                PdfPageBase page = pdf.Pages.Add(size);

                //Draw the image at a specified location on the page
                page.Canvas.DrawImage(pdfImg, 0, 0, width, height);
            }

            //Save the result file
            pdf.SaveToFile(@"C:\\Dev\Shmia\AMAApi\bin\TiffToPdf.pdf");

            return Ok("true");
        }

        public static Image[] SplitTiffImage(Image tiffImage)
        {
            //Get the number of frames in the Tiff image
            int frameCount = tiffImage.GetFrameCount(FrameDimension.Page);
            //Create an image array to store the split tiff images
            Image[] images = new Image[frameCount];
            //Gets the GUID of the first frame dimension
            Guid objGuid = tiffImage.FrameDimensionsList[0];
            //Create a FrameDimension object
            FrameDimension objDimension = new FrameDimension(objGuid);

            //Iterate through each frame
            for (int i = 0; i < frameCount; i++)
            {
                //Select a specified frame
                tiffImage.SelectActiveFrame(objDimension, i);
                //Save the frame in TIFF format to a memory stream
                MemoryStream ms = new MemoryStream();
                tiffImage.Save(ms, ImageFormat.Tiff);
                //Load an image from memory stream
                images[i] = Image.FromStream(ms);
            }
            return images;
        }




        [Authorize]
        [Route("getFarms")]
        [HttpGet]
        public IHttpActionResult GetFarms(bool deleted = false)
        {
            return Ok(FarmsService.GetFarms(deleted));
        }

        [Authorize]
        [Route("getFarm/{id}")]
        [HttpGet]
        public IHttpActionResult GetFarm(int id)
        {
            return Ok(FarmsService.GetFarm(id));
        }

        [Authorize]
        [Route("newFarm")]
        [HttpGet]
        public IHttpActionResult NewFarm()
        {
            return Ok(new Farm());
        }

        [Authorize]
        [Route("deleteFarm/{id}")]
        [HttpGet]
        public IHttpActionResult DeleteFarm(int id)
        {
            FarmsService.DeleteFarm(id);
            return Ok();
        }

        //[Authorize(Roles = "sysAdmin")]
        [Authorize]
        [Route("updateFarm")]
        [HttpPost]
        public IHttpActionResult UpdateFarm(Farm farm)
        {
            return Ok(FarmsService.UpdateFarm(farm));
        }

        [Authorize]
        [Route("updateFarmInvoice")]
        [HttpPost]
        public IHttpActionResult UpdateFarmInvoice(Farm farm)
        {
            return Ok();

        }

        [Authorize]
        [Route("getMangerFarm")]
        [HttpGet]
        public IHttpActionResult GetMangerFarm()
        {
            return Ok(FarmsService.GetMangerFarm());
        }


        [Authorize]
        [Route("getMangerInstructorFarm")]
        [HttpGet]
        public IHttpActionResult GetMangerInstructorFarm()
        {
            return Ok(FarmsService.GetMangerInstructorFarm());
        }


        [Authorize]
        [Route("setMangerInstructorFarm")]
        [HttpPost]
        public IHttpActionResult SetMangerInstructorFarm(List<FarmInstructors> FarmInstructors)
        {
            return Ok(FarmsService.SetMangerInstructorFarm(FarmInstructors));
        }


        [Authorize]
        [Route("setMangerFarm")]
        [HttpPost]
        public IHttpActionResult SetMangerFarm(FarmManagers farmmanger)
        {
            return Ok(FarmsService.SetMangerFarm(farmmanger));
        }

        [Authorize]
        [Route("getFarmsMainUser/{FarmId}")]
        [HttpGet]
        public IHttpActionResult GetFarmsMainUser(int FarmId)
        {
            return Ok(FarmsService.GetFarmsMainUser(FarmId));
        }


        [Authorize]
        [Route("getFarmPDFFiles/{id}")]
        [HttpGet]
        public IHttpActionResult GetFarmPDFFiles(int id)
        {
            using (var Context = new Context())
            {
                List<FarmPDFFiles> FarmPDFFiles = Context.FarmPDFFiles.Where(x => x.FarmId == id && x.StatusId == 1).OrderBy(x => x.Seq).ToList();


                return Ok(FarmPDFFiles);
            }


        }

        [Authorize]
        [Route("updateFarmsPdfFiles/{type}/{id}")]
        [HttpPost]
        public IHttpActionResult UpdateFarmsPdfFiles(int type, int id, List<FarmPDFFiles> farmPDFFiles)
        {
            //הכנסה של 101 בלבד
            if (type == 2)
            {
                using (var Context = new Context())
                {
                    var FarmPDFFilesList = Context.FarmPDFFiles.Where(x => x.FarmId == id && x.StatusId == 1).ToList();


                    FarmPDFFiles farmPDFFile = new FarmPDFFiles();
                    farmPDFFile.FarmId = id;
                    farmPDFFile.StatusId = 1;
                    farmPDFFile.FileName = "101.pdf";
                    farmPDFFile.Is101 = true;

                    if (FarmPDFFilesList.Count > 0)
                        farmPDFFile.Seq = FarmPDFFilesList[0].Seq + 1;
                    else
                        farmPDFFile.Seq = 1;


                    bool IsExistFileName = FarmPDFFilesList.Any(x => x.Is101);

                    if (!IsExistFileName)
                    {

                        Context.FarmPDFFiles.Add(farmPDFFile);
                        Context.SaveChanges();
                    }

                    FarmPDFFilesList = Context.FarmPDFFiles.Where(x => x.FarmId == id).ToList();

                    return Ok(FarmPDFFilesList);

                }
            }

            //מחיקה
            if (type == 3)
            {
                using (var Context = new Context())
                {
                    FarmPDFFiles fpf = farmPDFFiles.Where(x => x.Id == id).FirstOrDefault();

                    if (fpf != null)
                    {
                        int FarmId = fpf.FarmId;
                        string FileName = fpf.FileName;

                        Context.FarmPDFFiles.Attach(fpf);

                        Context.FarmPDFFiles.Remove(fpf);
                        Context.SaveChanges();

                        if (!fpf.Is101)
                        {
                            var root = HttpContext.Current.Server.MapPath("~/Uploads/Companies/" + FarmId.ToString() + "/PDF/" + FileName);

                            if (File.Exists(root))
                            {
                                File.Delete(root);
                            }
                        }


                        var FarmPDFFilesList = Context.FarmPDFFiles.Where(x => x.FarmId == FarmId).ToList();

                        return Ok(FarmPDFFilesList);
                    }
                }
            }

            //שינוי סדר מסמכים
            if (type == 4)
            {
                using (var Context = new Context())
                {
                    foreach (var item in farmPDFFiles)
                    {
                        Context.Entry(item).State = System.Data.Entity.EntityState.Modified;
                    }

                    Context.SaveChanges();
                }



            }
          
            return Ok();
        }
        //[Authorize]
        //[Route("getKlalitHistoris")]
        //[HttpGet]

        //public IHttpActionResult GetKlalitHistoris(int FarmId , string startDate = null, string endDate = null,int? type=null,int? klalitId = null)
        //{
        //    return Ok(FarmsService.GetKlalitHistoris(FarmId, startDate, endDate,type,klalitId));
        //}


        //[Authorize]
        //[Route("setKlalitHistoris")]
        //[HttpPost]

        //public IHttpActionResult SetKlalitHistoris(KlalitHistoris kh)
        //{
        //    return Ok(FarmsService.SetKlalitHistoris(kh));
        //}


    }
}
