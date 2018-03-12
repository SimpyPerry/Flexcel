using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using Domain;
using LINQtoCSV;
using System.IO;

namespace DataAccess
{
    public class CSVExportToCallList
    {

        Encoding encoding;
        //*Ændring af FilePath til filePath, ihenhold til navngivningskonventioner.
        string filePath;
        ListContainer listContainer;
        List<Offer> winningOfferList;
        public CSVExportToCallList(string filePath)
        {
            //*ændring af filePath til this.filePath så navngivning passer og variablene ikke bliver byttet rundt
            this.filePath = filePath;
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
                if (File.Exists(filePath))
                {
                    // Note that no lock is put on the
                    // file and the possibility exists
                    // that another process could do
                    // something with it between
                    // the calls to Exists and Delete.
                    //*FilePath rettelse
                    File.Delete(filePath);
                }
                // Create the file.
                //*FilePath rettelse
                using (StreamWriter streamWriter = new StreamWriter(@filePath, true, encoding)) 
                {
                    streamWriter.WriteLine("Nummer" + ";" + "Virksomhedsnavn" + ";" + "Navn" + ";" +"Vedståede v. 2" + ";" + "Vedståede v. 3" + ";" + "Vedståede v. 5" + ";" + "Vedståede v. 6" + ";"+ "Vedståede v. 7" + ";" + "Vundne v. 2"+";" + "Vundne v. 3" + ";" + "Vundne v. 5" +";" + "Vundne v. 6" +";" + "Vundne v. 7");
                    List<Offer> offersToPrint = new List<Offer>(); 

                    foreach (Offer offer in winningOfferList)
                    { 
                        //* tjekker om UserID i objektet er det samme som UserID i offer
                        if (!offersToPrint.Any(obj => obj.UserID == offer.UserID))
                        {
                            offersToPrint.Add(offer);
                            streamWriter.WriteLine(offer.OfferReferenceNumber + ";" + offer.Contractor.CompanyName + ";" + offer.Contractor.ManagerName + ";" + offer.Contractor.NumberOfType2PledgedVehicles + ";" + offer.Contractor.NumberOfType3PledgedVehicles + ";" + offer.Contractor.NumberOfType5PledgedVehicles + ";" + offer.Contractor.NumberOfType6PledgedVehicles + ";" + offer.Contractor.NumberOfType7PledgedVehicles + ";" + offer.Contractor.NumberOfWonType2Offers + ";" + offer.Contractor.NumberOfWonType3Offers + ";" + offer.Contractor.NumberOfWonType5Offers + ";" + offer.Contractor.NumberOfWonType6Offers + ";" + offer.Contractor.NumberOfWonType7Offers + ";");

                        }
                    }
                    streamWriter.Close();
                }

                // Open the stream and read it back.
                //*FilePath rettelse
                //*Ændring af sr til streamReader, og s til streamReaderstring, for bedre navngivning.
                using (StreamReader streamReader = File.OpenText(filePath))
                {
                    string streamReaderstring = "";
                    while ((streamReaderstring = streamReader.ReadLine()) != null)
                    {
                        Console.WriteLine(streamReaderstring);
                    }
                }
            }
            catch(Exception)
            {
                throw new Exception("Filen blev ikke gemt");
            }
        }
            
        
    }
}