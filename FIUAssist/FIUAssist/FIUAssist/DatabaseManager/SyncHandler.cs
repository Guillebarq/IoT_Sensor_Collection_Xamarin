using FIUAssist.Utils;
using FIUAssist.Views;
using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.Sync;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace FIUAssist.DatabaseManager
{
    public class SyncHandler : IMobileServiceSyncHandler
    {
        private string _log;
        public async Task<JObject> ExecuteTableOperationAsync(IMobileServiceTableOperation operation)
        {
            try
            {
                Debug.Write(string.Format("Execute operation: {0}    {1}", operation.Kind, operation.Item));
                _log = string.Format("Execute operation: {0}    {1}", operation.Kind.ToString(), operation.Item.ToString());
                if (InsertionLog.insertLog != null)
                {
                    InsertionLog.insertLog(_log);
                }
                return await operation.ExecuteAsync();
            }
            catch (MobileServiceConflictException ex)
            {
                await operation.Table.UpdateAsync(operation.Item);
                ExceptionErrorLogger.writeFileOnInternalStorage(ex.ToString());
                return new JObject();
            }
        }

        public Task OnPushCompleteAsync(MobileServicePushCompletionResult result)
        {
            foreach (var error in result.Errors)
                if (error.Status == HttpStatusCode.Conflict)
                {
                    error.CancelAndUpdateItemAsync(error.Result);
                    error.Handled = true;
                }
            return Task.FromResult(0);
        }


    }
}
