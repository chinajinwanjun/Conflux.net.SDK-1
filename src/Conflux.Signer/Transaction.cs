﻿using System;
using System.Numerics;
using System.Threading.Tasks;
using Conflux.Hex.HexConvertors.Extensions;
using Conflux.Model;
using Conflux.RLP;

namespace Conflux.Signer
{
    public class Transaction : SignedTransactionBase
    {
        public object chainId { get; set; }
        public object v { get; set; }
        public object r { get; set; }
        public object s { get; set; }
        public object epochHeight { get; set; }
        public object storageLimit { get; set; }

        public Transaction(byte[] rawData)
        {
            SimpleRlpSigner = new RLPSigner(rawData, NUMBER_ENCODING_ELEMENTS);
            ValidateValidV(SimpleRlpSigner);
        }

        public Transaction(RLPSigner rlpSigner)
        {
            ValidateValidV(rlpSigner);
            SimpleRlpSigner = rlpSigner;
        }

        private static void ValidateValidV(RLPSigner rlpSigner)
        {
            if (rlpSigner.IsVSignatureForChain())
                throw new Exception("TransactionChainId should be used instead of Transaction");
        }

        public Transaction(byte[] nonce, byte[] gasPrice, byte[] gasLimit, byte[] receiveAddress, byte[] value,
            byte[] data)
        {
            SimpleRlpSigner = new RLPSigner(GetElementsInOrder(nonce, gasPrice, gasLimit, receiveAddress, value, data));
        }

        public Transaction(byte[] nonce, byte[] gasPrice, byte[] gasLimit, byte[] receiveAddress, byte[] value,
            byte[] data, byte[] r, byte[] s, byte v)
        {
            SimpleRlpSigner = new RLPSigner(GetElementsInOrder(nonce, gasPrice, gasLimit, receiveAddress, value, data),
                r, s, v);
        }

        public Transaction(string to, BigInteger amount, BigInteger nonce)
            : this(to, amount, nonce, DEFAULT_GAS_PRICE, DEFAULT_GAS_LIMIT)
        {
        }

        public Transaction(string to, BigInteger amount, BigInteger nonce, string data)
            : this(to, amount, nonce, DEFAULT_GAS_PRICE, DEFAULT_GAS_LIMIT, data)
        {
        }

        public Transaction(string to, BigInteger amount, BigInteger nonce, BigInteger gasPrice, BigInteger gasLimit)
            : this(to, amount, nonce, gasPrice, gasLimit, "")
        {
        }

        public Transaction(string to, BigInteger amount, BigInteger nonce, BigInteger gasPrice,
            BigInteger gasLimit, string data) : this(nonce.ToBytesForRLPEncoding(), gasPrice.ToBytesForRLPEncoding(),
            gasLimit.ToBytesForRLPEncoding(), to.HexToByteArray(), amount.ToBytesForRLPEncoding(), data.HexToByteArray()
        )
        {
            this.to = to.HexToByteArray();
            this.Data = data.HexToByteArray();
            this.Gas = gasLimit.ToBytesForRLPEncoding();

        }

        public string ToJsonHex()
        {
            var s = "['{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}']";
            return string.Format(s, Nonce.ToHex(),
                GasPrice.ToHex(), Gas.ToHex(), to.ToHex(), Value.ToHex(), ToHex(Data),
                Signature.V.ToHex(),
                Signature.R.ToHex(),
                Signature.S.ToHex());
        }

        private byte[][] GetElementsInOrder(byte[] nonce, byte[] gasPrice, byte[] gasLimit, byte[] receiveAddress,
            byte[] value,
            byte[] data)
        {
            if (receiveAddress == null)
                receiveAddress = DefaultValues.EMPTY_BYTE_ARRAY;
            //order  nonce, gasPrice, gasLimit, receiveAddress, value, data
            return new[] { nonce, gasPrice, gasLimit, receiveAddress, value, data };
        }

        public override CfxECKey Key => CfxECKey.RecoverFromSignature(SimpleRlpSigner.Signature, SimpleRlpSigner.RawHash);

#if !DOTNET35
        public override async Task SignExternallyAsync(IEthExternalSigner externalSigner)
        {
            await externalSigner.SignAsync(this).ConfigureAwait(false);
        }
#endif
    }
}