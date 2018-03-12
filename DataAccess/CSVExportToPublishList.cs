using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Domain; 
using System.Threading.Tasks;
using System.IO;

namespace DataAccess
{
    public class CSVExportToPublishList
    {
        Encoding encoding;
        //*Ændring af FilePath til filePath, ihenhold til navngivning konventioner.
        string _filePath;
        ListContainer listContainer;
        List<Offer> winningOfferList;

        
        public CSVExportToPublishList(string filePath)
        {
            //*ændring af filePath til this.filePath så navngivning passer og variablene ikke bliver byttet rundt
            _filePath = filePath;
            listContainer = ListContainer.GetInstance();
            winningOfferList = listContainer.outputList;
            encoding = Encoding.GetEncoding("iso-8859-1");
        }
        public void CreateFile()
        {
            try
            {
                // Delete the file if it exists.
                //*FilePath rettelse
                if (File.Exists(_filePath))
                {
                    // Note that no lock is put on the
                    // file and the possibility exists
                    // that another process could do
                    // something with it between
                    // the calls to Exists and Delete.
                    //*FilePath rettelse
                    File.Delete(_filePath);
                }
                // Create the file.
                //*FilePath rettelse
                using (StreamWriter streamWriter = new StreamWriter(@_filePath, true, encoding))
                {
                    streamWriter.WriteLine("Garantivognsnummer" + ";" + "Virksomhedsnavn" + ";" + "Pris" + ";");
                    foreach (Offer offer in winningOfferList)
                    {
                        streamWriter.WriteLine(offer.RouteID + ";" + offer.Contractor.CompanyName + ";" + offer.OperationPrice + ";");
                    }
                    streamWriter.Close();
                }

                // Open the stream and read it back.
                //*FilePath rettelse
                //*Ændring af sr til streamReader, og s til streamReaderstring, for bedre navngivning.
                using (StreamReader streamReader = File.OpenText(_filePath))
                {
                    string streamReaderstring = "";
                    while ((streamReaderstring = streamReader.ReadLine()) != null)
                    {
                        Console.WriteLine(streamReaderstring);
                    }
                }
            }
            catch (Exception)
            {
                throw new Exception("Filen blev ikke gemt");
            }
        }

    }

}
