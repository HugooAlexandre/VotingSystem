@echo off
echo ===========================================
echo SISTEMA DE VOTAÇÃO ELETRÓNICA
echo Ano letivo 2025-2026
echo ===========================================
echo.

echo [1/4] Verificando estrutura...
if not exist "Shared\Protos\" mkdir Shared\Protos
if not exist "Shared\Protos\voter.proto" (
    echo Erro: Ficheiro voter.proto não encontrado!
    pause
    exit /b 1
)

echo [2/4] Compilando VotingApp...
cd VotingApp
dotnet restore
dotnet build
cd ..

echo [3/4] Compilando RegistoClient...
cd RegistoClient
dotnet restore
dotnet build
cd ..

echo [4/4] Compilando VotacaoClient...
cd VotacaoClient
dotnet restore
dotnet build
cd ..

echo.
echo ===========================================
echo COMPILAÇÃO CONCLUÍDA COM SUCESSO!
echo ===========================================
echo.
echo Para executar:
echo 1. VotingApp (completo):    cd VotingApp && dotnet run
echo 2. RegistoClient (AR):      cd RegistoClient && dotnet run
echo 3. VotacaoClient (AV):      cd VotacaoClient && dotnet run
echo.
echo ===========================================
echo ENDPOINT: ken01.utad.pt:9091
echo ===========================================
pause