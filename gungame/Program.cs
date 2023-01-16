using Raylib_cs;

//vars
int speed = 200;
bool host = false;

//pre game start
Console.WriteLine("host(0) or client(1)?");
if (Console.ReadLine() == "0")
{
    netcode.Host();
    host = true;
}
else
{
    netcode.Connect();
}

player p1 = new player(0, 0, 50, 50);
player p2 = new player(0, 0, 50, 50);


Thread netUpdate = new Thread(async () =>
{
    while (true)
    {
        var posTemp = await netcode.Receive();
        p2.x = (int)posTemp.X;
        p2.y = (int)posTemp.Y;
        netcode.Send(p1.x, p1.y);
        Thread.Sleep(30);
    }
});

//create window
Raylib.InitWindow(500, 500, "game");
Raylib.SetTargetFPS(60);

//Main gameloop
while (!Raylib.WindowShouldClose())
{
    if (netcode.connected)
    {
        if (host)
        {
            Console.WriteLine("Client connected.");
            netUpdate.Start();
            netcode.connected = false;
        }
        else
        {
            Console.WriteLine("Connected to Server");
            netcode.Send(p1.x, p1.y);
            netUpdate.Start();
            netcode.connected = false;
        }
    }

    p1.x += (int)((Raylib.IsKeyDown(KeyboardKey.KEY_D) - Raylib.IsKeyDown(KeyboardKey.KEY_A)) * speed * Raylib.GetFrameTime());
    p1.y += (int)((Raylib.IsKeyDown(KeyboardKey.KEY_S) - Raylib.IsKeyDown(KeyboardKey.KEY_W)) * speed * Raylib.GetFrameTime());

    Raylib.BeginDrawing();
    Raylib.ClearBackground(Color.DARKGRAY);
    Raylib.DrawRectangle(p2.x, p2.y, 50, 50, Color.GREEN);
    Raylib.DrawRectangle(p1.x, p1.y, 50, 50, Color.RED);
    Raylib.EndDrawing();
}