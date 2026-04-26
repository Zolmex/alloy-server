using Common.Database;
using Common.Database.Models;

namespace Common.Network.Messaging.Impl;

public record struct GetAccountAck : IAppMessageAck {
    public GetAccountAck(int seq) {
        Sequence = seq;
    }

    public Account Account { get; set; }
    public AppMessageId MessageId => AppMessageId.GetAccount;
    public int Sequence { get; set; }

    public void Write(ref SpanWriter wtr) {
        var hasValue = Account != null;
        wtr.Write(hasValue);
        if (hasValue)
            Account.WriteProperties(ref wtr);
    }

    public void Read(ref SpanReader rdr) {
        Account = DbModel.Read<Account>(ref rdr);
    }
}