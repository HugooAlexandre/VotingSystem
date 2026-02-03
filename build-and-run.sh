#!/bin/bash

echo "==========================================="
echo "SISTEMA DE VOTAÇÃO ELETRÓNICA"
echo "Ano letivo 2025-2026"
echo "==========================================="
echo ""

echo "[1/4] Verificando estrutura..."
if [ ! -d "Shared/Protos" ]; then
    echo "Erro: Pasta Shared/Protos não encontrada!"
    exit 1
fi

if [ ! -f "Shared/Protos/voter.proto" ]; then
    echo "Erro: Ficheiro voter.proto não encontrado!"
    exit 1
fi

echo "[2/4] Compilando VotingApp..."
cd VotingApp
dotnet restore
dotnet build
cd ..

echo "[3/4] Compilando RegistoClient..."
cd RegistoClient
dotnet restore
dotnet build
cd ..

echo "[4/4] Compilando VotacaoClient..."
cd VotacaoClient
dotnet restore
dotnet build
cd ..

echo ""
echo "==========================================="
echo "COMPILAÇÃO CONCLUÍDA COM SUCESSO!"
echo "==========================================="
echo ""
echo "Para executar:"
echo "1. VotingApp (completo):    cd VotingApp && dotnet run"
echo "2. RegistoClient (AR):      cd RegistoClient && dotnet run"
echo "3. VotacaoClient (AV):      cd VotacaoClient && dotnet run"
echo ""
echo "==========================================="
echo "ENDPOINT: ken01.utad.pt:9091"
echo "==========================================="
read -p "Pressione Enter para continuar..."