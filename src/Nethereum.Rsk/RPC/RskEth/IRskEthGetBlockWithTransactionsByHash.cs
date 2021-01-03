using System.Threading.Tasks;
using Conflux.JsonRpc.Client;
using Conflux.Rsk.RPC.RskEth.DTOs;

namespace Conflux.Rsk.RPC.RskEth
{
    public interface IRskEthGetBlockWithTransactionsByHash
    {
        Task<RskBlockWithTransactions> SendRequestAsync(string blockHash, object id = null);
        RpcRequest BuildRequest(string blockHash, object id = null);
    }
}