using System.Diagnostics;

string proj = "";
string[] paths = new string[3];
paths[0] = Path.Combine("..", "AbcPharmacy", "AbcPharmacy.csproj");
paths[1] = Path.Combine("..", "..", "..", "..", "AbcPharmacy", "AbcPharmacy.csproj");
paths[2] = Path.Combine(Directory.GetCurrentDirectory(), "AbcPharmacy", "AbcPharmacy.csproj");

for (int i = 0; i < paths.Length; i++)
{
    string p = Path.GetFullPath(paths[i]);
    if (File.Exists(p))
    {
        proj = p;
        break;
    }
}

if (proj == "")
{
    DirectoryInfo di = new DirectoryInfo(AppContext.BaseDirectory);
    while (di != null)
    {
        string p = Path.Combine(di.FullName, "AbcPharmacy", "AbcPharmacy.csproj");
        if (File.Exists(p))
        {
            proj = p;
            break;
        }
        di = di.Parent;
    }
}

if (proj == "")
{
    Console.WriteLine("AbcPharmacy not found");
    return;
}

Console.WriteLine(proj);
Console.WriteLine("http://localhost:5088");

ProcessStartInfo psi = new ProcessStartInfo();
psi.FileName = "dotnet";
psi.Arguments = "run --project \"" + proj + "\" --urls http://localhost:5088";
psi.UseShellExecute = false;

Process p1 = Process.Start(psi);
if (p1 != null)
    p1.WaitForExit();
