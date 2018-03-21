using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Domain;
using System.IO;
using System.Globalization;
namespace DataAccess
{
    public class CSVImport
    {

        Encoding encoding;
        public List<Contractor> listOfContractors;
        public List<RouteNumber> listOfRouteNumbers;
        public List<Offer> listOfOffers;
        public CSVImport()
        {
            listOfContractors = new List<Contractor>();
            listOfRouteNumbers = new List<RouteNumber>();
            listOfOffers = new List<Offer>();
            encoding = Encoding.GetEncoding("iso-8859-1");
        }
        public int TryParseToIntElseZero(string toParse)
        {
            int number;
            toParse = toParse.Replace(" ", "");
            bool tryParse = Int32.TryParse(toParse, out number);
            return number;
        }
        public float TryParseToFloatElseZero(string toParse)
        {
            //* CurrentCultureName -> currentCultureName
            string currentCultureName = Thread.CurrentThread.CurrentCulture.Name;
            CultureInfo cultureInformation = new CultureInfo(currentCultureName);
            if (cultureInformation.NumberFormat.NumberDecimalSeparator != ",")
            // Forcing use of decimal separator for numerical values
            {
                cultureInformation.NumberFormat.NumberDecimalSeparator = ",";
                Thread.CurrentThread.CurrentCulture = cultureInformation;
            }
            float number;
            toParse = toParse.Replace(" ", "");
            bool tryParse = float.TryParse(toParse.Replace('.', ','), out number);
            return number;
        }
        //*filepath til filePath
        public void ImportOffers(string filePath)
        {
            try
            {
                //*filepath til filePath
                //*mere overskuelig struktur
                var data = File.ReadAllLines(filePath, encoding)
                .Skip(1).Select(x => x.Split(';')).Select(x => new Offer
                {
                    OfferReferenceNumber = x[0],
                    RouteID = TryParseToIntElseZero(x[1]),
                    OperationPrice = TryParseToFloatElseZero((x[2])),
                    UserID = x[5],
                    GarantiedHours= TryParseToFloatElseZero((x[6])),
                    GarantiedDays = TryParseToFloatElseZero((x[7])),
                    CreateRouteNumberPriority = x[8],
                    CreateContractorPriority = x[9],
                });
                //* var o -> Offer offer
                foreach (Offer offer in data)
                {

                    if (offer.UserID != "" || offer.OperationPrice != 0)
                    {          
                        offer.RouteNumberPriority = TryParseToIntElseZero(offer.CreateRouteNumberPriority);
                        offer.ContractorPriority = TryParseToIntElseZero(offer.CreateContractorPriority);
                        //* x -> contractorID
                        Contractor contractor = listOfContractors.Find(contractorID => contractorID.UserID == offer.UserID);
                        try
                        {
                            //* r -> routeNumberID
                            offer.RequiredVehicleType = (listOfRouteNumbers.Find(routeNumberID => routeNumberID.RouteID == offer.RouteID)).RequiredVehicleType;
                            Offer newOffer = new Offer(offer.OfferReferenceNumber, offer.OperationPrice,offer.GarantiedHours,offer.GarantiedDays, offer.RouteID, offer.UserID, offer.RouteNumberPriority, offer.ContractorPriority, contractor, offer.RequiredVehicleType);
                            listOfOffers.Add(newOffer);
                        }
                        catch
                        {
                            // Help for debugging purpose only.
                            string failure = offer.RouteID.ToString();
                        }
                    }

                }
                foreach (RouteNumber routeNumber in listOfRouteNumbers)
                {
                    foreach (Offer offer in listOfOffers)
                    {
                        if (offer.RouteID == routeNumber.RouteID)
                        {
                            routeNumber.offers.Add(offer);
                        }
                    }
                }
            }
            catch (IndexOutOfRangeException)
            {
                throw new IndexOutOfRangeException("Fejl, er du sikker på du har valgt den rigtige fil?");
            }
            catch (FormatException)
            {
                throw new FormatException("Fejl, er du sikker på du har valgt den rigtige fil?");
            }
            catch (Exception)
            {
                throw new Exception("Fejl, filerne blev ikke importeret");
            }
        }
        public void ImportRouteNumbers()
        {
            try
            {
                //* filepath -> filePath
                //* structur rettelse
                string filePath = Environment.ExpandEnvironmentVariables("RouteNumbers.csv");
                var data = File.ReadAllLines(filePath, encoding).Skip(1).Select(x => x.Split(';')).Select(x => new RouteNumber
                {
                    RouteID = TryParseToIntElseZero(x[0]),
                    RequiredVehicleType = TryParseToIntElseZero(x[1]),
                });
                foreach (RouteNumber routeNumber in data)
                {
                    bool doesAlreadyContain = listOfRouteNumbers.Any(obj => obj.RouteID == routeNumber.RouteID);

                    if (!doesAlreadyContain && routeNumber.RouteID != 0 && routeNumber.RequiredVehicleType != 0)
                    {
                        listOfRouteNumbers.Add(routeNumber);
                    }
                }

            }


            catch (IndexOutOfRangeException)
            {
                throw new IndexOutOfRangeException("Fejl, er du sikker på du har valgt den rigtige fil?");
            }
            catch (FormatException)
            {
                throw new FormatException("Fejl, er du sikker på du har valgt den rigtige fil?");
            }
            catch (Exception)
            {
                throw new Exception("Fejl, filerne blev ikke importeret");
            }
        }
        //* filepath -> filePath
        public void ImportContractors(string filePath)
        {
            try
            {
                //* filepath -> filePath
                //* Structur rettelse
                var data = File.ReadAllLines(filePath, encoding).Skip(1).Select(x => x.Split(';')).Select(x => new Contractor                                                    
                {
                      ReferenceNumberBasicInformationPDF = x[0],
                      ManagerName = x[1],
                      CompanyName = x[2],
                      UserID = x[3],        
                      TryParseValueType2PledgedVehicles = x[4],
                      TryParseValueType3PledgedVehicles = x[5],
                      TryParseValueType5PledgedVehicles = x[6],
                      TryParseValueType6PledgedVehicles = x[7],
                      TryParseValueType7PledgedVehicles = x[8],

                });
                //*var c -> Contrator contractor
                foreach (Contractor contractor in data)
                {
                    if (contractor.UserID != "")
                    {
                        {
                            bool doesAlreadyContain = listOfContractors.Any(obj => obj.UserID == contractor.UserID);

                            contractor.NumberOfType2PledgedVehicles = TryParseToIntElseZero(contractor.TryParseValueType2PledgedVehicles);
                            contractor.NumberOfType3PledgedVehicles = TryParseToIntElseZero(contractor.TryParseValueType3PledgedVehicles);
                            contractor.NumberOfType5PledgedVehicles = TryParseToIntElseZero(contractor.TryParseValueType5PledgedVehicles);
                            contractor.NumberOfType6PledgedVehicles = TryParseToIntElseZero(contractor.TryParseValueType6PledgedVehicles);
                            contractor.NumberOfType7PledgedVehicles = TryParseToIntElseZero(contractor.TryParseValueType7PledgedVehicles);

                            Contractor newContractor = new Contractor(contractor.ReferenceNumberBasicInformationPDF, contractor.UserID, contractor.CompanyName, contractor.ManagerName, contractor.NumberOfType2PledgedVehicles, contractor.NumberOfType3PledgedVehicles, contractor.NumberOfType5PledgedVehicles, contractor.NumberOfType6PledgedVehicles, contractor.NumberOfType7PledgedVehicles);
                            listOfContractors.Add(newContractor);
                        }
                    }
                }
            }
            catch (IndexOutOfRangeException)
            {
                throw new IndexOutOfRangeException("Fejl, er du sikker på du har valgt den rigtige fil?");
            }
            catch (FormatException)
            {
                throw new FormatException("Fejl, er du sikker på du har valgt den rigtige fil?");
            }
            catch (Exception)
            {
                throw new Exception("Fejl, filerne blev ikke importeret");
            }
        }
        public List<Contractor> SendContractorListToContainer()
        {
            return listOfContractors;
        }
        public List<RouteNumber> SendRouteNumberListToContainer()
        {
            return listOfRouteNumbers;
        }
    }
}
