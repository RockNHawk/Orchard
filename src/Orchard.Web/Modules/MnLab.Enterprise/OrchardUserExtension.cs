using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MnLab.Enterprise;
using MnLab.Enterprise.Approval;
using MnLab.Enterprise.Approval.Models;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Records;
using Orchard.Security;
using Orchard.Users.Models;
using Rhythm.Diagnostics;
using Rhythm.EventSourcing;

namespace Rhythm {


    //class FakeIEventTrace : IEventTrace {
    //    public List<IEventTrace> Childs => throw new NotImplementedException();

    //    public IEvent Event => throw new NotImplementedException();

    //    public TraceObject TraceObject => throw new NotImplementedException();

    //    public Action<IEventTrace> OnBeginTrace { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    //    public EventTrace<TEvent> BeginTrace<TEvent>(TEvent eventObj) where TEvent : IEvent {
    //        return this;
    //    }

    //    public EventTrace<TEvent> BeginTrace<TEvent>(TEvent eventObj, EventBusBase eventBus) where TEvent : IEvent {
    //        return this;
    //    }

    //    public void Debugging(string message, Exception ex = null) {
    //        throw new NotImplementedException();
    //    }

    //    public Exception Error(string message, Exception ex = null) {
    //        throw new NotImplementedException();
    //    }

    //    public Exception Error(Exception ex) {
    //        throw new NotImplementedException();
    //    }

    //    public Exception Error(string errorCategory, string errorCode, Exception ex) {
    //        throw new NotImplementedException();
    //    }

    //    public Exception Error(string errorCategory, string errorCode, string message, Exception ex) {
    //        throw new NotImplementedException();
    //    }

    //    public Exception Error(TraceInfo info) {
    //        throw new NotImplementedException();
    //    }

    //    public void Success() {
    //        throw new NotImplementedException();
    //    }

    //    public void Warning(string message, Exception ex = null) {
    //        throw new NotImplementedException();
    //    }

    //    #region IDisposable Support
    //    private bool disposedValue = false; // To detect redundant calls

    //    protected virtual void Dispose(bool disposing) {
    //        if (!disposedValue) {
    //            if (disposing) {
    //                // TODO: dispose managed state (managed objects).
    //            }

    //            // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
    //            // TODO: set large fields to null.

    //            disposedValue = true;
    //        }
    //    }

    //    // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
    //    // ~FakeIEventTrace() {
    //    //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
    //    //   Dispose(false);
    //    // }

    //    // This code added to correctly implement the disposable pattern.
    //    public void Dispose() {
    //        // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
    //        Dispose(true);
    //        // TODO: uncomment the following line if the finalizer is overridden above.
    //        // GC.SuppressFinalize(this);
    //    }
    //    #endregion

    //}

    /// <summary>
    /// </summary>
    public static class OrchardUserExtension {

        public static ContentItem GetContentItem(this ContentItemRecord record, IContentManager contentManager) {
            return contentManager.Get(record.Id, VersionOptions.AllVersions);
        }

        public static UserPartRecord User(this Orchard.WorkContext wc) {
            return wc.CurrentUser.As<UserPart>().Record;
        }

        public static IEventTrace BeginTrace(this Orchard.WorkContext wc, IEvent @event) {
            // return null;
            //return new DisposableProxy(()=> { });
            //return new EventTrace<IEvent>(@event, new Rhythm.WorkContext());
            return new EventTrace<IEvent>(@event, null);
        }

        //public static IDisposable BeginTrace(this Orchard.WorkContext wc, IEvent @event) {
        //    // return null;
        //    return new DisposableProxy(() => { });
        //}

        public static Exception Error(this IDisposable ds, Exception ex) {
            // return null;
            return ex;
        }

        public static DepartmentRecord Department(this UserPartRecord user1) {
            return null;
        }

        public static string Username(this UserPartRecord user1) {
            return user1.UserName;
        }

        public static string Username(this IUser user1) {
            /// http://derekdevdude.blogspot.com/2012/10/accessing-orchard-current-user-content.html
            //userContent.As<ProfilePart>();
            //if (userContent.ProfilePart != null &&
            //  userContent.ProfilePart.Has(typeof(object), "DisplayName") &&
            //  userContent.ProfilePart.DisplayName.Value is string) {
            //    displayName = userContent.ProfilePart.DisplayName.Value.Trim();
            //}
            //if (String.IsNullOrWhiteSpace(displayName)) {
            return user1.UserName;
        }



    }
}