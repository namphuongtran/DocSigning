using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace iDocsSign
{
    class Program
    {
        const string usageText = "";
        [STAThread]
        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to E-Sign System.");

            /* gets value from params
                //      /s      sign
                //      /spdf   pdf source file
                //      /tpdf   pdf target file
                //      /pfx    pfx file
                //      /p      password pfx file
            */
            bool isSign = false;
            // Attempt to PDF source file.
            string pdfSourceFile = string.Empty;
            //  Attempt to PDF target file.
            string pdfTargetFile = string.Empty;
            // pfx file.
            string pfxFile = string.Empty;
            // password pfx file
            string passwordPfx = string.Empty;

            /*int step = 0;
            do
            {
                if (step == 0)
                {
                    Console.WriteLine("Enter \"pdf source file path\".");
                    pdfSourceFile = Console.ReadLine();
                }
                if (step == 1)
                {
                    Console.WriteLine("Enter \"pdf target file path\".");
                    pdfTargetFile = Console.ReadLine();
                }
                if (step == 2)
                {
                    Console.WriteLine("Enter \"pfx certificate file path\".");
                    pfxFile = Console.ReadLine();
                }
                if (step == 3)
                {
                    Console.WriteLine("Enter \"pfx certificate password\".");
                    passwordPfx = Utility.ReadPassword();
                }
                step++;
            } while (step < 4);*/

            try
            {
                for (int i = 0; i < args.Length; i++)
                {
                    if (args[i].ToLower().Equals("/?"))
                    {
                        isSign = false;
                        break;
                    }
                    switch (args[i].ToLower())
                    {
                        case "/s":
                            isSign = true;
                            break;
                        case "/spdf":
                            pdfSourceFile = args[i + 1];
                            i++;
                            break;
                        case "/tpdf":
                            pdfTargetFile = args[i + 1];
                            i++;
                            break;
                        case "/pfx":
                            pfxFile = args[i + 1];
                            i++;
                            break;
                        case "/p":
                            passwordPfx = args[i + 1];
                            i++;
                            break;
                    }
                }
            }
            catch
            {
                Console.WriteLine("The syntax of the command is incorrect");
                Console.WriteLine();
                ShowHelp();
                //return;
            }
            if (!string.IsNullOrEmpty(pdfSourceFile)
                && !string.IsNullOrEmpty(pdfTargetFile)
                && !string.IsNullOrEmpty(pfxFile)
                && !string.IsNullOrEmpty(passwordPfx))
            {
                Sign(pdfSourceFile, pdfTargetFile, pfxFile, passwordPfx);
                Console.ReadLine();
            }
            else
            {
                Console.WriteLine("The syntax of the command is incorrect");
                Console.WriteLine();
                ShowHelp();
                //return;
            }
        }

        static void Sign(string pdfSourceFile, string pdfTargetFile, string pfxFile, string passwordPfx)
        {
            /*pdfSourceFile = AppDomain.CurrentDomain.BaseDirectory + pdfSourceFile;
            pdfTargetFile = AppDomain.CurrentDomain.BaseDirectory + pdfTargetFile;
            pfxFile = AppDomain.CurrentDomain.BaseDirectory + pfxFile;*/
            bool hasPdfSourceFile = File.Exists(pdfSourceFile);
            bool hasPdfTargetFile = File.Exists(pdfTargetFile);
            bool hasPfxFile = File.Exists(pfxFile);
            if (hasPdfSourceFile && hasPfxFile)
            {
                string pdfSourceExtension = Path.GetExtension(pdfSourceFile);
                string pdfTargetExtension = Path.GetExtension(pdfTargetFile);
                string pfxExtension = Path.GetExtension(pfxFile);
                if (pdfSourceExtension == ".pdf" && pdfTargetExtension == ".pdf" && pfxExtension == ".pfx")

                {
                    try
                    {
                        Console.WriteLine("Started ....");
                        Console.WriteLine("Checking certificate ...");

                        Cert myCert = null;
                        try
                        {
                            myCert = new Cert(pfxFile, passwordPfx);
                            Console.WriteLine("Certificate OK");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Error : please make sure you entered a valid certificate file and password");
                            Console.WriteLine("Exception : " + ex.ToString());
                            Logger.WriteError("Error : please make sure you entered a valid certificate file and password", ex);
                            //return;
                        }

                        string author = string.Empty;
                        string title = string.Empty;
                        string subject = string.Empty;
                        string keyword = string.Empty;
                        string creator = string.Empty;
                        string producer = string.Empty;
                        MetaData MyMD = new MetaData();
                        try
                        {
                            PdfReader reader = new PdfReader(pdfSourceFile);

                            MetaData md = new MetaData();
                            md.Info = reader.Info;

                            author = md.Author;
                            title = md.Title;
                            subject = md.Subject;
                            keyword = md.Keywords;
                            creator = md.Creator;
                            producer = md.Producer;

                            //Adding Meta Datas                    
                            MyMD.Author = author;
                            MyMD.Title = title;
                            MyMD.Subject = subject;
                            MyMD.Keywords = keyword;
                            MyMD.Creator = creator;
                            MyMD.Producer = producer;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Exception : " + ex.ToString());
                            //return;
                        }

                        Console.WriteLine("Signing document ... ");
                        PDFSigner pdfs = new PDFSigner(pdfSourceFile, pdfTargetFile, myCert, MyMD);
                        pdfs.Sign("", "", "", false);

                        Console.WriteLine("Done!!!");
                    }
                    catch (IOException e)
                    {
                        TextWriter errorWriter = Console.Error;
                        errorWriter.WriteLine(e.Message);
                        errorWriter.WriteLine(usageText);
                        //return;
                    }
                }
                else
                {
                    Console.WriteLine("Incorect file format!");
                    Console.WriteLine();
                }
            }
            else
            {
                Console.WriteLine("File is not existed!");
                Console.WriteLine();
            }
        }

        static void ShowHelp()
        {
            string applicationName = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;

            System.Text.StringBuilder cmdBuilder = new System.Text.StringBuilder();
            cmdBuilder.Append("USAGE:");
            cmdBuilder.AppendLine();
            cmdBuilder.AppendFormat("{0} /s [/? | /spdf sourcepdf |", applicationName);
            cmdBuilder.AppendLine();
            cmdBuilder.AppendLine("          /tpdf targetpdf | /pfx pfxfile | /p password]");
            cmdBuilder.AppendLine("where");
            cmdBuilder.AppendLine("     sourcepdf                Source PDF file: input PDF file");
            cmdBuilder.AppendLine("     targetpdf                Target PDF file: output PDF file");
            cmdBuilder.AppendLine("     pfxfile                  PFX file");
            cmdBuilder.AppendLine("     password                 Password of PFX file");
            cmdBuilder.AppendLine();
            cmdBuilder.AppendLine();
            cmdBuilder.AppendLine("The option /u must be specified to run app in update mode. If other options are not specified, default values will be used.");
            cmdBuilder.AppendLine();
            cmdBuilder.AppendLine("Examples:");
            cmdBuilder.AppendFormat("     > {0} /s /spdf unsigned.pdf /tpdf singed.pdf /pfx secret.pfx /p 123456", applicationName);

            Console.WriteLine(cmdBuilder.ToString());
        }
    }
}
