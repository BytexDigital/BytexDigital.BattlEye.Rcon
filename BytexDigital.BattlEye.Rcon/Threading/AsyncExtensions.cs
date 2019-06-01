﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BytexDigital.BattlEye.Rcon.Threading {
    // Source: https://stackoverflow.com/questions/19404199/how-to-to-make-udpclient-receiveasync-cancelable/24148785
    public static class AsyncExtensions {
        public static async Task<T> WithCancellation<T>(this Task<T> task, CancellationToken cancellationToken) {
            var tcs = new TaskCompletionSource<bool>();

            using (cancellationToken.Register(s => ((TaskCompletionSource<bool>)s).TrySetResult(true), tcs)) {
                if (task != await Task.WhenAny(task, tcs.Task)) {
                    throw new OperationCanceledException(cancellationToken);
                }
            }

            return task.Result;
        }

        public static async Task WithCancellation(this Task task, CancellationToken cancellationToken) {
            var tcs = new TaskCompletionSource<bool>();

            using (cancellationToken.Register(s => ((TaskCompletionSource<bool>)s).TrySetResult(true), tcs)) {
                if (task != await Task.WhenAny(task, tcs.Task)) {
                    throw new OperationCanceledException(cancellationToken);
                }
            }

            return;
        }
    }
}
