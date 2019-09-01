using FIUAssist.Utils;
using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.SQLiteStore;
using Microsoft.WindowsAzure.MobileServices.Sync;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace FIUAssist.DatabaseManager
{
    public class SensorDataService
    {
        const string applicationURL = @"https://sensorloggerapp.azurewebsites.net";
        public IMobileServiceSyncTable<SmartPhoneSensors> phoneSensorTable;
        public IMobileServiceSyncTable<WoundSensors> woundSensorTable;
        public IMobileServiceSyncTable<SmartWatchSensors> watchSensorTable;

        static MobileServiceSQLiteStore store;

        static MobileServiceClient client;

        private static SensorDataService _instance;

        public static SensorDataService Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new SensorDataService();
                }
                return _instance;
            }
        }

        public MobileServiceClient CurrentClient
        {
            get
            {

                if (client == null || client.SyncContext == null)
                {
                    _instance = new SensorDataService();                   
                }

                return client;
            }
        }

        private SensorDataService()
        {
            client = new MobileServiceClient(applicationURL);
            Initialize();
        }

        string path = "";
        public async Task Initialize()
        {
            try
            {
                switch (Xamarin.Forms.Device.RuntimePlatform)
                {
                    case Xamarin.Forms.Device.iOS:
                        path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "..", "Library", "FIUAssist.db");
                        break;
                    case Xamarin.Forms.Device.Android:
                        path = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "FIUAssistDB.db ");
                        //CopyDatabase(path);
                        break;
                    default:
                        throw new NotImplementedException("Platform not supported");

                }

                if (!File.Exists(path))
                {
                    File.Create(path).Dispose();
                }

                store = new MobileServiceSQLiteStore(path);
                store.DefineTable<SmartPhoneSensors>();
                store.DefineTable<WoundSensors>();
                store.DefineTable<SmartWatchSensors>();

                this.phoneSensorTable = this.CurrentClient.GetSyncTable<SmartPhoneSensors>();
                this.woundSensorTable = this.CurrentClient.GetSyncTable<WoundSensors>();
                this.watchSensorTable = this.CurrentClient.GetSyncTable<SmartWatchSensors>();

                await client.SyncContext.InitializeAsync(store, new SyncHandler());

            }
            catch (Exception ex)
            {
                ExceptionErrorLogger.writeFileOnInternalStorage(ex.ToString());
            }

        }

    }
}
