
using ApplicationCore.Enums;
using ApplicationCore.Interfaces.Registries;
using ApplicationCore.Registries;
using Infrastructure.EntityFramework;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Implementation.Repositories.Registries
{
    public class NumeratorRepository : INumeratorRepository
    {

        private ApplicationDbContext db;
        public NumeratorRepository(ApplicationDbContext context)
        {
            db = context;
        }

        public string GetNextStringNumber(ObjectTypes objectType, long CustomerID, DateTime dateIssue)
        {
            var IsYearPeriod = true; //TODO
            var MaskNumber = "000000";//TODO put to settings document
            var CurrentNumber = db.Numerators.FirstOrDefault(n => n.ObjectType == objectType && n.SellerID == CustomerID
            && (n.DateIssue.Year == dateIssue.Year || !n.IsYearPeriod));
            if (CurrentNumber == null)
            {
                var newNumerator = new Numerator();
                newNumerator.SellerID = CustomerID;
                newNumerator.DateIssue = dateIssue.ToUniversalTime();
                newNumerator.ObjectType = objectType;
                newNumerator.IsYearPeriod = IsYearPeriod;
                newNumerator.CurrentStringNumber = "000001";
                db.Numerators.Add(newNumerator);
                db.SaveChanges();
                return newNumerator.CurrentStringNumber;
            }
            else
            {
                int next = int.Parse(CurrentNumber.CurrentStringNumber!.TrimStart('0')) + 1;

                //string nextString = (int.Parse(MaskNumber.TrimStart('0')) + 1).ToString();
                //int addedZero = MaskNumber.Length - nextString.Length;

                CurrentNumber.CurrentStringNumber = next.ToString("D" + MaskNumber.Length);
                db.Numerators.Update(CurrentNumber);
                db.SaveChanges();
                return CurrentNumber.CurrentStringNumber;

            }

        }

        public long GetNextLongNumber(ObjectTypes objectType, long CustomerID, DateTime dateIssue)
        {
            var IsYearPeriod = true; //TODO

            var CurrentNumber = db.Numerators.FirstOrDefault(n => n.ObjectType == objectType && n.SellerID == CustomerID
            && (n.DateIssue.Year == dateIssue.Year || !n.IsYearPeriod));
            if (CurrentNumber == null)
            {
                var newNumerator = new Numerator();
                newNumerator.SellerID = CustomerID;
                newNumerator.DateIssue = dateIssue;
                newNumerator.ObjectType = objectType;
                newNumerator.IsYearPeriod = IsYearPeriod;
                newNumerator.CurrentLongNumber = 1;
                db.Numerators.Add(newNumerator);
                db.SaveChangesAsync();
                return (long)newNumerator.CurrentLongNumber;
            }
            else
            {


                CurrentNumber.CurrentLongNumber++;
                db.Numerators.Update(CurrentNumber);
                db.SaveChangesAsync();
                return (long)CurrentNumber.CurrentLongNumber!;

            }

        }

        public IEnumerable<Numerator> GetList()
        {
            return db.Numerators.ToList();
        }


    }
}
