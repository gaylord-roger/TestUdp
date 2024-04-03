var srv = new UpdServer(80);

Console.WriteLine("Ecoute en UDP sur le port 80");
Console.Read();

srv.Stop();