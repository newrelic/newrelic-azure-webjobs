# newrelic-azure-webjobs
This is a demo project that illustrates how to use the New Relic .NET agent to instrument Azure Web Jobs.  Learn more about Azure Web Jobs [here](https://azure.microsoft.com/en-us/documentation/articles/websites-webjobs-resources/)

###Steps

**(1)** Add the New Relic Site Extension to your host site:
See the docs under "Install using the Site Extension from the SCM site"

**(2)** Restart the Site (notice you will not see anything report to New Relic because we have not added your license key yet)

**(3)** Get your license key (browse to https://rpm.newrelic.com/accounts/[accountId] after logging in - the license key will appear on the right side)

**(4)** In the app.config of the webjob add the following appsetting

```
<configuration>
  <appSettings>
    <add key="NewRelic.AppName" value="[replace_with_the_name_you_want_reported_to_new_relic]" />
    <add key="NewRelic.AgentEnabled" value="true" />
    <add key="NewRelic.LicenseKey" value="[replace_with_your_key]" />
  </appSettings> 
</configuration> 

```

**(5)** Finally add some instrumentation (i.e. help the agent look for what you feel is important to monitor)

 - In the webjob add a folder and file: newrelic\extensions\\[someuniquename].xml
 - Make sure it's Build action is Content
 - In the file add your instrumentation

For instance - let's say in your function.cs you have:

```
namespace WebJobs.Journals
{
    public class Functions
    {
        public static void JournalOperations(parms...)
        {
            new JobRunHandler<JobMessage>(parms...)
                .Run(parms..);
        }

    }
}
```

The content in the xml instrumentation file will look like:

```
<?xml version="1.0" encoding="utf-8"?>
<extension xmlns="urn:newrelic-extension">
  <instrumentation>
    <tracerFactory name="NewRelic.Agent.Core.Tracer.Factories.BackgroundThreadTracerFactory">
      <match assemblyName="WebJobs.Journals" className="WebJobs.Journals.Functions">
        <exactMethodMatcher methodName="JournalOperations" />
      </match>
    </tracerFactory>
  </instrumentation>
</extension>
```

**(6)** Restart your Host site


###Some things to note:
You'll still see the host site being reported to New Relic - this is because the profiler sees the W3WP process and will automatically attach to it.

The default configuration for transactions being recorded is **2 seconds**. Many webjobs functions might execute faster than the established **2 seconds**. This means you might not see any transactions unless the function exceeds the **2 second** threshold.  You can modify the value in the newrelic.config (found in configuration > ```transactionTracer``` > ```transactionThreshold``` attribute).  

You might want to set this value to something like **.10** (which is equivalent to 100 ms).

```
<transactionTracer enabled="true" transactionThreshold=".10" stackTraceThreshold="1" recordSql="obfuscated" explainEnabled="true" explainThreshold="500" />
```

The best way to set this value is to (there is an example of this in the source in this repository):

- Navigate to: https://[yourwebhost].scm.azurewebsites.net/DebugConsole
Site >> wwwroot >> newrelic >> download newrelic.config
- Add this file to the "host site" (not the webjob's root) by adding a newrelic folder and dropping the newrelic.config in the folder with the build action as "content"
- Publish the host site.

This will allow the web site extension to take what you have put in this folder and merge / override the default content from the extension itself.

If you want to take advantage of some of the Agent APIs in your web job simply install the New Relic Agent API nuget package into your project: 

``` PM> Install-Package NewRelic.Agent.Api ```
