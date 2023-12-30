using System.Collections.Generic;
using Solana.Unity.Programs;
using Solana.Unity.Rpc.Models;
using Solana.Unity.SDK;
using Solana.Unity.Soar;
using Solana.Unity.Soar.Program;
using Solana.Unity.Soar.Types;
using Solana.Unity.Wallet;
using TMPro;
using UnityEngine;

public class SoarInit : MonoBehaviour
{
    [Header("Game")]
    public TMP_InputField gameTitle;
    public TMP_InputField gameDescription;
    public TMP_InputField gameGenre;
    public TMP_InputField gameType;
    public TMP_InputField gameNftMeta;
    public TextMeshProUGUI gameOutput;

    [Header("Leaderboard")]
    public TMP_InputField gamePubkey;
    public TMP_InputField leaderboardNftMeta;
    public TMP_InputField leaderboardDescription;
    public TMP_InputField leaderboardScoresToRetain;
    public TMP_Dropdown leaderboardScoresOrder;
    public TMP_Dropdown leaderboardMultilpeScore;
    public TextMeshProUGUI leaderboardOutput;

    public async void SoarGameInit()
    {
        var tx = new Transaction()
        {
            FeePayer = Web3.Account,
            Instructions = new List<TransactionInstruction>(),
            RecentBlockHash = await Web3.BlockHash()
        };
        var game = new Account();
        
        var gameMeta = new GameAttributes()
        {
            Title = gameTitle.text,
            Description = gameDescription.text,
            Genre = byte.Parse(gameGenre.text),
            GameType = byte.Parse(gameType.text),
            NftMeta = new PublicKey(gameNftMeta.text)
        };

        var initializeGameAccounts = new InitializeGameAccounts()
        {
            Creator = Web3.Account,
            Game = game,
            SystemProgram = SystemProgram.ProgramIdKey
        };

        var initializeGameIx = SoarProgram.InitializeGame(
            accounts: initializeGameAccounts,
            gameMeta: gameMeta,
            gameAuth: new[] { Web3.Account.PublicKey },
            SoarProgram.ProgramIdKey
        );
        tx.Add(initializeGameIx);


        tx.PartialSign(Web3.Account);
        tx.PartialSign(game);


        var res = await Web3.Wallet.SignAndSendTransaction(tx);
        string txMsg = $"https://solscan.io/tx/{res.Result}{(Web3.Instance.rpcCluster == RpcCluster.DevNet ? "?cluster=devnet" : Web3.Instance.rpcCluster == RpcCluster.TestNet ? "?cluster=testnet" : "")}";
        Debug.Log(txMsg);
        gameOutput.text = txMsg;
    }

    public async void SoarLeaderboardInit()
    {
        var tx = new Transaction()
        {
            FeePayer = Web3.Account,
            Instructions = new List<TransactionInstruction>(),
            RecentBlockHash = await Web3.BlockHash()
        };
        var game = new PublicKey(gamePubkey.text);
        var soarClient = new SoarClient(Web3.Rpc, Web3.WsRpc);
        var gameAccount = (await soarClient.GetGameAsync(game)).ParsedResult;
        var id = gameAccount.LeaderboardCount + 1;

        var leaderboard = SoarPda.LeaderboardPda(game, id);
        var topEntries = SoarPda.LeaderboardTopEntriesPda(leaderboard);
        var leaderboardMeta = new RegisterLeaderBoardInput()
        {
            Description = leaderboardDescription.text,
            NftMeta = new PublicKey(leaderboardNftMeta.text),
            ScoresToRetain = byte.Parse(leaderboardScoresToRetain.text),
            IsAscending = leaderboardScoresOrder.value == 0,
            AllowMultipleScores = leaderboardMultilpeScore.value == 0
        };

        var addLeaderboardAccounts = new AddLeaderboardAccounts()
        {
            Authority = Web3.Account,
            Payer = Web3.Account,
            Game = game,
            Leaderboard = leaderboard,
            TopEntries = topEntries,
            SystemProgram = SystemProgram.ProgramIdKey
        };
        var createLeaderboardIx = SoarProgram.AddLeaderboard(
            accounts: addLeaderboardAccounts,
            input: leaderboardMeta,
            SoarProgram.ProgramIdKey
        );
        tx.Add(createLeaderboardIx);

        var res = await Web3.Wallet.SignAndSendTransaction(tx);
        string txMsg = $"https://solscan.io/tx/{res.Result}{(Web3.Instance.rpcCluster == RpcCluster.DevNet ? "?cluster=devnet" : Web3.Instance.rpcCluster == RpcCluster.TestNet ? "?cluster=testnet" : "")}";
        Debug.Log(txMsg);
        leaderboardOutput.text = txMsg;
    }
}