using SIML.FTP.CustomExceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tamir.SharpSsh;
using Tamir.SharpSsh.jsch;
using System.IO;

namespace SIML.FTP
{
    public static class SFTP
    {
        


        public static bool Send(string ftpAddress, string username, string password, string port, string sourceFolderPath, string filename, string archiveFolderPath)
        {
            try
            {
                //these are for copying files to an archive
                string sourceFile = System.IO.Path.Combine(sourceFolderPath, filename);
                string sendFile = System.IO.Path.Combine(archiveFolderPath, filename);
                try
                {
                    if (!System.IO.Directory.Exists(filename))
                    {

                        throw new FileNotFoundException();
                    }
                    else if (System.IO.Directory.Exists(filename))
                    {
                        throw new IOException("File Already Exists");
                    }
                }
                catch (FileNotFoundException e)
                {
                    MessageBox.Show(e.Message.ToString());
                }
                //Copying process
                System.IO.File.Copy(sourceFile, sendFile, true);

                bool Success = false;
                Sftp sftp = null;
                try
                {

                    if (filename.Length > 0)
                    {

                        int NumberOfConnectionAttempts = 0;

                        int TotalAllowedConnectionAttempts = 3;

                        //Here is the jump point for the application to go back
                        JumpPoint:

                        try
                        {
                            //Here we are passing credentials and FTP Address
                            sftp = new Sftp(ftpAddress, username);

                            sftp.Password = password;

                            sftp.Connect();

                            sftp.Put(filename, (!String.IsNullOrWhiteSpace(sourceFolderPath) ? sourceFolderPath + "/" : "") + filename);

                            Success = true;

                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(" Connection " + NumberOfConnectionAttempts + " failed.\n");

                            if (NumberOfConnectionAttempts < TotalAllowedConnectionAttempts)
                            {
                                NumberOfConnectionAttempts++;

                                Thread.Sleep(1000);

                                //Here we jump back after a failed connection
                                goto JumpPoint;
                            }
                        }
                    }

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                    MessageBox.Show("Something went wrong.");
                }
                finally
                {

                    try { sftp.Close(); }
                    catch { }

                    try { sftp = null; }
                    catch { }

                    try { GC.Collect(); }
                    catch { }

                }
                return Success;
            }
            catch (Exception ex)
            {
                throw new FolderException("There was a problem with the file directory.");
            }
        }
       
            
    }
}
