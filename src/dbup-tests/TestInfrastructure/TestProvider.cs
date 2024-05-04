using DbUp.Builder;
using DbUp.Engine.Transactions;
using DbUp.Tests.Common;
using DbUp.Tests.Common.RecordingDb;

namespace DbUp.Tests.TestInfrastructure;

public class TestProvider
{
    public InMemoryJournal Journal { get; } 
    public UpgradeEngineBuilder Builder { get; } = new();
    public CaptureLogsLogger Log { get; } = new();
    public RecordingDbConnection Connection { get; }

    public TestProvider(string schema)
        : this(connectionManager: null, schema: schema)
    {        
    }

    public TestProvider(IConnectionManager connectionManager)
    : this(connectionManager: connectionManager, schema: null)
    {
    }

    public TestProvider(IConnectionManager connectionManager = null, string schema = "dbo")
    {
        Journal = new InMemoryJournal(Log);
        Connection = new RecordingDbConnection(Log);
        Builder.Configure(c => c.ConnectionManager = connectionManager ?? new TestConnectionManager(Connection));
        Builder.JournalTo(Journal);
        Builder.Configure(c => c.ScriptExecutor = new TestScriptExecutor(c, schema));
        Builder.LogTo(Log);
    }
}
