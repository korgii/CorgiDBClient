using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dropbox.Api;
using System.Windows.Forms;
using System.IO;
using Dropbox.Api.Files;
using System.Drawing;

namespace WindowsFormsApplication1
{
    class DBClient
    {
        public static Form1 thisForm;
        public static Dropbox.Api.Users.FullAccount fullAccountInstance;
        public static string accessToken;
        public static DropboxClient client;

        public static void RunDropboxApi()
        {
            try
            {
                thisForm = Application.OpenForms["Form1"] as Form1;
                var task = Task.Run((Func<Task>)DBClient.Run);
                task.Wait();

                thisForm.SetLabel1Text = fullAccountInstance.Name.DisplayName;
                thisForm.SetLabel2Text = fullAccountInstance.Email;
                thisForm.SetLabel3Text = fullAccountInstance.Country;
                BeforeListRootFolder();
            }
            catch (Exception){
                return;
            }

        }

        static async Task Run()
        {
            using (var dbx = new DropboxClient(accessToken))
            {
                try{
                    fullAccountInstance = await dbx.Users.GetCurrentAccountAsync();
                    client = dbx;
                }
                catch (Exception e){
                    MessageBox.Show(e.Message);
                    return;
                }
            }
        }

        public async static void BeforeListRootFolder()
        {
            await ListRootFolder(client);
        }

        public static async Task ListRootFolder(DropboxClient dbx)
        {
            var list = await dbx.Files.ListFolderAsync(string.Empty);

            List<string> filestringList = new List<string>();
            List<string> folderstringList = new List<string>();

            list.Entries.Where(i => i.IsFolder).ToList().ForEach(s => folderstringList.Add(s.Name));
            list.Entries.Where(i => i.IsFile).ToList().ForEach(s => filestringList.Add(s.Name));

            thisForm.ListFilesToComboBox(filestringList, thisForm.GetComboBox2);
            thisForm.ListFilesToComboBox(folderstringList, thisForm.GetComboBox1);
        }

        public async static void BeforeDownload()
        {
            var folder = String.Empty;
            var file = thisForm.GetComboBox2.Text;

            await Download(client, folder, file);
        }

        static async Task Download(DropboxClient dbx, string folder, string file)
        {
            using (var response = await dbx.Files.DownloadAsync(folder + "/" + file))
            {
                var byteArray = await response.GetContentAsByteArrayAsync();
                using (MemoryStream mStream = new MemoryStream(byteArray))
                {
                    var hoh = Image.FromStream(mStream);
                    //hoh.Save("c:\\Image1", System.Drawing.Imaging.ImageFormat.Png);
                    //hoh.Save(Application.StartupPath + "\\img.jpg");
                    thisForm.ChangePictureBoxImage = hoh;
                }
            }

            //try
            //{
            //    //var response = await dbx.Files.DownloadAsync(folder + "/" + file);
            //    using (var response = await dbx.Files.DownloadAsync(folder + "/" + file))
            //    {
            //        ulong fileSize = response.Response.Size;
            //        const int bufferSize = 1024 * 1024;

            //        var buffer = new byte[bufferSize];

            //        using (var stream = await response.GetContentAsStreamAsync())
            //        {
            //            using (var fileStreamFile = new FileStream("Test", FileMode.OpenOrCreate))
            //            {
            //                var length = stream.Read(buffer, 0, bufferSize);

            //                while (length > 0)
            //                {
            //                    fileStreamFile.Write(buffer, 0, length);
            //                    var percentage = 100 * (ulong)file.Length / fileSize;
            //                    // Update progress bar with the percentage.
            //                    // progressBar.Value = (int)percentage
            //                    Console.WriteLine(percentage);

            //                    length = stream.Read(buffer, 0, bufferSize);
            //                }
            //            }
            //        }
            //    }

            //}
            //catch (Exception e)
            //{
            //    Console.WriteLine("EERRROOOOR:" + e.Message);
            //}

        }

        public static async Task Upload(DropboxClient dbx, string folder, string file, string content)
        {
            using (var mem = new MemoryStream(Encoding.UTF8.GetBytes(content)))
            {
                var updated = await dbx.Files.UploadAsync(
                    folder + "/" + file,
                    WriteMode.Overwrite.Instance,
                    body: mem);
                Console.WriteLine("Saved {0}/{1} rev {2}", folder, file, updated.Rev);
            }
        }
    }
}