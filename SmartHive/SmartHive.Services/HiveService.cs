﻿using SmartHive.Services.Contracts;
using System;
using System.Collections.Generic;
using SmartHive.Models;
using SmartHive.Data.Contracts;
using System.Linq;
using SmartHive.Factories;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SmartHive.Services.JsonModels;

namespace SmartHive.Services
{
    public class HiveService : IHiveService
    {
        private readonly IRepository<Hive> hiveRepository;
        private readonly IUnitOfWork unitOfWork;
        private readonly IHiveFactory hiveFactory;
        private readonly string uri = "https://api.thingspeak.com/channels/279981/feeds.json?api_key=";

        public HiveService(
            IRepository<Hive> hiveRepository,
            IUnitOfWork unitOfWork,
            IHiveFactory hiveFactory)
        {
            if (hiveRepository == null)
            {
                throw new ArgumentNullException(nameof(hiveRepository));
            }

            if (unitOfWork == null)
            {
                throw new ArgumentNullException(nameof(unitOfWork));
            }

            if (hiveFactory == null)
            {
                throw new ArgumentNullException(nameof(hiveFactory));
            }

            this.hiveRepository = hiveRepository;
            this.unitOfWork = unitOfWork;
            this.hiveFactory = hiveFactory;
        }

        public Hive CreateHive(string name, string dataKey, string userId)
        {
            var hive = this.hiveFactory.CreateHive(name, dataKey, userId);

            this.hiveRepository.Add(hive);
            this.unitOfWork.Commit();

            return hive;
        }

        public void EditHive(int id, string name, string dataKey)
        {
            var hive = this.hiveRepository.GetById(id);

            if (hive != null)
            {
                hive.Name = name;
                hive.DataKey = dataKey;

                this.hiveRepository.Update(hive);
                this.unitOfWork.Commit();
            }
        }

        public IEnumerable<Hive> GetAllHives()
        {
            return this.hiveRepository.GetAll.ToList();
        }

        public IList<JsonHive> GetHive(string dataKey)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                Task<string> response = httpClient.GetStringAsync($"{uri}{dataKey}");

                JObject json = JObject.Parse(response.Result);

                IList<JToken> results = json["feeds"].Children().ToList();

                IList<JsonHive> hives = new List<JsonHive>();

                foreach(var result in results)
                {
                    JsonHive hiveRes = result.ToObject<JsonHive>();
                    hives.Add(hiveRes);
                }

                return hives;
            }
        }

        public Hive GetHiveById(int id)
        {
            return this.hiveRepository.GetById(id);
        }

        public IEnumerable<Hive> GetHivesByUserId(string id)
        {
            return this.hiveRepository.GetAll.Where(h => h.UserId == id).ToList();
        }
    }
}
