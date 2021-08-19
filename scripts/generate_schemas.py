import subprocess
import argparse
argparser = argparse.ArgumentParser()
argparser.add_argument("-d", "--dotnet", default="dotnet", help="Specifies the .NET Core command")
args = argparser.parse_args()
subprocess.call([args.dotnet, "build", "-c", "Release"])
subprocess.call([args.dotnet, "src/SchemaGenerator/bin/Release/net5.0/SchemaGenerator.dll", "-d", "schemas"])