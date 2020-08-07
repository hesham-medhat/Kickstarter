using Kickstarter.Data;
using Kickstarter.Data.Models;
using Microsoft.EntityFrameworkCore;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace Kickstarter.Core.Services
{
    public static class UtilsService
    {
        /// <summary>
        /// Returns all countries available
        /// </summary>
        /// <returns>countries list as json object</returns>
        public static async Task<string> GetCountries()
        {
            var dbContext = new SQLDbContext();
            var countries = await dbContext.Country.Select(c => new {c.CountryId ,c.Name, c.IsoCode2, c.IsoCode3}).ToListAsync();
            return JsonConvert.SerializeObject(countries);
        }

        /// <summary>
        /// Returns all zones of a country
        /// </summary>
        /// <returns>countries list as json object</returns>
        public static async Task<string> GetZones(int countryId)
        {
            var dbContext = new SQLDbContext();
            var zones = await dbContext.Zone.Where(z => z.CountryId == countryId).Select(z => new {z.ZoneId ,z.Name}).ToListAsync();
            return JsonConvert.SerializeObject(zones);
        }

        /// <summary>
        /// Returns all categories available
        /// </summary>
        /// <returns>Categories list as json object</returns>
        public static async Task<string> GetCategories()
        {
            var dbContext = new SQLDbContext();
            var categories = await dbContext.Category.Select(c => new {c.Category1}).ToListAsync();
            List<string> categoriesList = new List<string>();
            foreach (var i in categories) {
                categoriesList.Add(i.Category1);
            }
            return JsonConvert.SerializeObject(categoriesList);
        }
    }

}
