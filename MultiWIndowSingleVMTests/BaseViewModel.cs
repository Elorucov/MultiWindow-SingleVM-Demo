﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;

namespace MultiWIndowSingleVMTests {
    public abstract class BaseViewModel : INotifyPropertyChanged {
        private readonly object _lock = new object();
        private readonly Dictionary<SynchronizationContext, PropertyChangedEventHandler> _handlersWithContext = new Dictionary<SynchronizationContext, PropertyChangedEventHandler>();
        public event PropertyChangedEventHandler PropertyChanged {
            add {
                if(value == null) {
                    return;
                }
                var synchronizationContext = SynchronizationContext.Current;
                lock(_lock) {
                    if(_handlersWithContext.TryGetValue(synchronizationContext, out PropertyChangedEventHandler eventHandler)) {
                        eventHandler += value;
                        _handlersWithContext[synchronizationContext] = eventHandler;
                    } else {
                        _handlersWithContext.Add(synchronizationContext, value);
                    }
                }
            }
            remove {
                if(value == null) {
                    return;
                }
                var synchronizationContext = SynchronizationContext.Current;
                lock(_lock) {
                    if(_handlersWithContext.TryGetValue(synchronizationContext, out PropertyChangedEventHandler eventHandler)) {
                        eventHandler -= value;
                        if(eventHandler != null) {
                            _handlersWithContext[synchronizationContext] = eventHandler;
                        } else {
                            _handlersWithContext.Remove(synchronizationContext);
                        }
                    }
                }
            }
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            KeyValuePair<SynchronizationContext, PropertyChangedEventHandler>[] handlersWithContext;
            lock(_lock) {
                handlersWithContext = _handlersWithContext.ToArray();
            }
            var eventArgs = new PropertyChangedEventArgs(propertyName);
            foreach(var handlerWithContext in handlersWithContext) {
                var synchronizationContext = handlerWithContext.Key;
                var eventHandler = handlerWithContext.Value;
                synchronizationContext.Post(o => eventHandler(this, eventArgs), null);
            }
        }
    }
}