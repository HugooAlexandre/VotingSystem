# Sistema de Votação Eletrónica

## Descrição
Sistema de votação eletrónica desenvolvido para a UC de Integração de Sistemas (Ano letivo 2025-2026), implementando uma arquitetura de três autoridades para garantir o anonimato do voto.

## Estrutura do Projeto
```text
VotingSystem/
├── Shared/Protos/ # Ficheiros .proto comuns
├── VotingApp/ # Aplicação principal completa
├── RegistoClient/ # Cliente especializado AR
├── VotacaoClient/ # Cliente especializado AV
├── README.md
├── build-and-run.bat # Script Windows
└── build-and-run.sh # Script Linux/macOS


## Funcionalidades

### Fase 1: Registo (AR)
- Validação do cartão de cidadão
- Emissão de credenciais únicas
- Garantia de elegibilidade

### Fase 2: Votação (AV)
- Listagem de candidatos
- Submissão de votos
- Validação de credenciais
- Prevenção de votos duplicados

### Fase 3: Resultados (AA)
- Contagem segura de votos
- Apresentação de resultados
- Garantia de anonimato

## Tecnologias Utilizadas
- .NET 7.0
- gRPC com TLS
- Protocol Buffers (protobuf)
- Arquitetura de 3 autoridades

## Configuração

### Pré-requisitos
- .NET 7.0 SDK ou superior
- Terminal/Command Prompt

### Instalação e Execução

#### Windows:
```bash
build-and-run.bat


Linux/macOS:

chmod +x build-and-run.sh
./build-and-run.sh

Execução Manual:

# Aplicação principal
cd VotingApp
dotnet run

# Cliente AR
cd RegistoClient
dotnet run

# Cliente AV
cd VotacaoClient
dotnet run

Endpoint dos Serviços
ken01.utad.pt:9091

Testes com grpcurl
Obter credencial:
'{ "citizen_card_number": "123456789"}' | grpcurl -insecure -proto Shared/Protos/voter.proto -d "@" ken01.utad.pt:9091 voting.VoterRegistrationService/IssueVotingCredential

Listar candidatos:
grpcurl -insecure -proto Shared/Protos/voting.proto ken01.utad.pt:9091 voting.VotingService/GetCandidates

Votar:

'{ "voting_credential": "CRED-ABC-123", "candidate_id":1}' | grpcurl -insecure -proto Shared/Protos/voting.proto -d "@" ken01.utad.pt:9091 voting.VotingService/Vote
