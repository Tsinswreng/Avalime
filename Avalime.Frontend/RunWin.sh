OUT_DIR=./proj/Avalime.Windows/bin/Debug/net10.0/win-x86
mkdir -p "$OUT_DIR"
cp ./proj/Avalime.Windows/Avalime.Ro.jsonc "$OUT_DIR/Avalime.Ro.jsonc"
cp ./proj/Avalime.Windows/Avalime.Rw.jsonc "$OUT_DIR/Avalime.Rw.jsonc"
dotnet run --project ./proj/Avalime.Windows/Avalime.Windows.csproj -r win-x86
