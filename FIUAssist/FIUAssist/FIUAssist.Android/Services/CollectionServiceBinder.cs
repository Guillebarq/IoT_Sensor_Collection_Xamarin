using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace FIUAssist.Droid.Services
{
    class CollectionServiceBinder: Binder
    {
        protected DatabaseInsertions service;

        public CollectionServiceBinder(DatabaseInsertions service)
        {
            this.service = service;
        }

        public DatabaseInsertions Service => service;

        public bool IsBound { get; set; }

    }
}