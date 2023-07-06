using System.Collections;
using System.Collections.Generic;

public class Location
{
    public int x;
    public int z;
    public Location()
    {
        x = 0;
        z = 0;
    }
    public Location(int _x, int _y)
    {
        x = _x;
        z = _y;
    }

    public static bool operator==(Location l1, Location l2)
    {
        return l1.x == l2.x && l1.z == l2.z;
    }
    public static bool operator!=(Location l1, Location l2)
    {
        return l1.x != l2.x || l1.z != l2.z;
    }
    public static Location operator+(Location l1, Location l2)
    {
        return new Location(l1.x + l2.x, l1.z + l2.z);
    }
    public static float Distance(Location l1, Location l2)
    {
        return MathF.Sqrt(MathF.Pow(l2.x - l1.x, 2) + MathF.Pow(l2.z - l1.z, 2));
    }
}

public class PathNode
{
    public Location location = new Location();
    public float G, H, F;
    public PathNode? parent = null;
    public string tag = "open";
    public PathNode(Location loc, float g, float h, float f, PathNode? prt)
    {
        location = loc;
        G = g;
        H = h;
        F = f;
        parent = prt;
    }

    public void Print(int line)
    {
        if (tag == "start")
        {
            Console.Write(" SSSSS ");
            return;
        }
        if (tag == "end")
        {
            Console.Write(" EEEEE ");
            return;
        }
        if (tag == "found")
        {
            Console.Write(" FFFFF ");
            return;
        }
        if (line == 0 || line == 4)
        {
            if (tag == "open")
                Console.Write(" OOOOO ");
            else if (tag == "closed")
                Console.Write(" XXXXX ");
            else
                Console.Write(" LLLLL ");
            return;
        }

        if (line == 1)
            Console.Write(" G" + G.ToString("0.00").Substring(0, 4) + " ");
        else if (line == 2)
            Console.Write(" H" + G.ToString("0.00").Substring(0, 4) + " ");
        else
            Console.Write(" F" + F.ToString("0.00").Substring(0, 4) + " ");
    }
}

public class FindPathAStar
{
    int mazeWidth = 12; //x length
    int mazeDepth = 10; //z length
    byte[,] mazeMap = {
        { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
        { 1, 0, 1, 0, 1, 1, 0, 1, 0, 1 },
        { 1, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
        { 1, 0, 1, 1, 0, 1, 1, 0, 1, 1 },
        { 1, 0, 1, 0, 1, 0, 0, 0, 0, 1 },
        { 1, 0, 1, 0, 0, 0, 1, 1, 0, 1 },
        { 1, 0, 1, 1, 1, 0, 1, 0, 0, 1 },
        { 1, 0, 0, 1, 0, 1, 0, 1, 1, 1 },
        { 1, 1, 0, 0, 0, 0, 0, 0, 0, 1 },
        { 1, 0, 0, 1, 1, 0, 1, 0, 0, 1 },
        { 1, 0, 0, 1, 1, 0, 0, 1, 0, 1 },
        { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
    };
    List<Location> directions = new List<Location>() {
                                    new Location(1,0),
                                    new Location(0,1),
                                    new Location(-1,0),
                                    new Location(0,-1)
                                };
    PathNode startNode;
    PathNode endNode;
    PathNode? lastNode;
    bool done = false;
    List<PathNode> openList = new List<PathNode>();
    List<PathNode> closedList = new List<PathNode>();

    public FindPathAStar()
    {
    }

    public void BeginSearch()
    {
        done = false;

        List<Location> locations = new List<Location>();

        for (int z = 1; z < mazeDepth - 1; ++z)
        {
            for (int x = 1; x < mazeWidth - 1; ++x)
            {
                if (mazeMap[x, z] != 1)
                {
                    locations.Add(new Location(x, z));
                }
            }
        }
        locations.Shuffle();

        startNode = new PathNode(new Location(locations[0].x, locations[0].z),
            0.0f, 0.0f, 0.0f, null);
        startNode.tag = "start";
        endNode = new PathNode(new Location(locations[1].x, locations[1].z),
            0.0f, 0.0f, 0.0f, null);
        endNode.tag = "end";
        openList.Clear();
        closedList.Clear();
        closedList.Add(startNode);
        Print();
        lastNode = startNode;
    }

    public void Search()
    {
        if (lastNode == null)
        {
            Console.WriteLine("Not started!");
            return;
        }
        if (lastNode.location == endNode.location)
        {
            done = true;
            Console.WriteLine("DONE!");
            PathNode? parent = lastNode.parent;
            while (parent != null && parent.location != startNode.location)
            {
                parent.tag = "found";
                parent = parent.parent;
            }
            Print();
            return;
        }

        if (lastNode != startNode)
            lastNode.tag = "closed";
        foreach (Location dir in directions)
        {
            Location neighbour = dir + lastNode.location;

            if (mazeMap[neighbour.x, neighbour.z] == 1)
                continue;
            if (neighbour.x < 1 || neighbour.x >= mazeWidth || neighbour.z < 1 || neighbour.z >= mazeDepth)
                continue;
            if (closedList.Find(n => n.location == neighbour) != null) // is in closedList
                continue;

            //float g = Location.Distance(thisNode.location, neighbour) + thisNode.G;
            float g = 1 + lastNode.G;
            float h = Location.Distance(neighbour, endNode.location);
            float f = g + h;

            if (!UpdateNode(neighbour, g, h, f, lastNode))
            {
                openList.Add(new PathNode(neighbour, g, h, f, lastNode));
            }
        }
        openList = openList.OrderBy(n => n.F).ThenBy(n => n.H).ToList<PathNode>();
        PathNode lowestFNode = openList[0];
        closedList.Add(lowestFNode);
        openList.RemoveAt(0);
        lastNode = lowestFNode;
        lastNode.tag = "last";
        Print();
    }

    bool UpdateNode(Location loc, float g, float h, float f, PathNode prt)
    {
        foreach (PathNode n in openList)
        {
            if (n.location == loc)
            {
                n.G = g;
                n.H = h;
                n.F = f;
                n.parent = prt;
                return true;
            }
        }
        return false;
    }

    public void Print()
    {
        for (int z = 0; z < mazeDepth; z++)
        {
            for (int line = 0; line < 5; line++)
            {
                for (int x = 0; x < mazeWidth; x++)
                {
                    if (mazeMap[x, z] == 1)
                    {
                        Console.Write("WWWWWWW");
                    }
                    else
                    {
                        var n = GetNode(x, z);
                        if (n != null)
                            n.Print(line);
                        else
                            Console.Write("       ");
                    }
                }
                Console.WriteLine();
            }
        }
    }

    PathNode? GetNode(int x, int z)
    {
        if (x == startNode.location.x && z == startNode.location.z)
            return startNode;
        if (x == endNode.location.x && z == endNode.location.z)
            return endNode;
        foreach (var n in openList)
        {
            if (x == n.location.x && z == n.location.z)
                return n;
        }
        foreach (var n in closedList)
        {
            if (x == n.location.x && z == n.location.z)
                return n;
        }
        return null;
    }
}
