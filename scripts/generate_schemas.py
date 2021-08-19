import subprocess
import argparse
argparser = argparse.ArgumentParser()
argparser.add_argument("-d", "--dotnet", default="dotnet", help="Specifies the .NET Core command")
args = argparser.parse_args()
subprocess.call([args.dotnet, "build", "src/SchemaGenerator/SchemaGenerator.csproj", "-c", "Release", "-o", "schemas/generator", "--nologo"])
subprocess.call([args.dotnet, "schemas/generator/SchemaGenerator.dll", "-d", "schemas"])