using Dental.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Dental.Models.Base
{
    public class ConnectToDb
    {
        public ConnectToDb()
        {
          /*  var builder = new ConfigurationBuilder();
            // установка пути к текущему каталогу
            builder..SetBasePath(Directory.GetCurrentDirectory());
            // получаем конфигурацию из файла appsettings.json
            builder.AddJsonFile("appsettings.json");
            // создаем конфигурацию
            var config = builder.Build();
            // получаем строку подключения
            string connectionString = config.GetConnectionString("DefaultConnection");

            var optionsBuilder = new DbContextOptionsBuilder<ApplicationContext>();
            var options = optionsBuilder.UseSqlite(connectionString).Options;*/




            Config = new Config();

            //if (Config.DbType == 1) Context = new PostgresContext(Config.ConnectionString); 

            //else Context = new SQLiteContext(Config.ConnectionString);
            Context = new ApplicationContext(Config.ConnectionString, Config.DbType);
        }

        public Config Config { get; private set; }
        public ApplicationContext Context { get; private set; }
    }
}
