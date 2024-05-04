using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using DbUp.Engine;

namespace DbUp.Builder;

/// <summary>
/// Builds a UpgradeEngine by accepting a list of callbacks to execute. For custom configuration, you should 
/// implement extension methods on top of this class.
/// </summary>
public class UpgradeEngineBuilder
{
    protected readonly ConcurrentQueue<Action<UpgradeConfiguration>> callbacks = new();

    /// <summary>
    /// Adds a callback that will be run to configure the upgrader when Build is called.
    /// </summary>
    /// <param name="configuration">The configuration.</param>
    public virtual void Configure(Action<UpgradeConfiguration> configuration)
    {
        callbacks.Enqueue(configuration);
    }

    /// <summary>
    /// Creates an UpgradeConfiguration based on this configuration.
    /// </summary>
    /// <returns></returns>
    protected virtual UpgradeConfiguration BuildConfiguration()
    {
        var config = new UpgradeConfiguration();

        while (callbacks.TryDequeue(out var callback) && callback is not null)
        {
            callback(config);
        }

        //foreach (var callback in callbacks)
        //{
        //    callback(config);
        //}

        config.Validate();

        return config;
    }

    /// <summary>
    /// Creates an UpgradeEngine based on this configuration.
    /// </summary>
    /// <returns></returns>
    public virtual UpgradeEngine Build()
    {
        var config = BuildConfiguration();
        return new UpgradeEngine(config);
    }
}
