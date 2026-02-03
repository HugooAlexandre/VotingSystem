# Ficheiros Protocol Buffer (.proto)

Estes ficheiros definem os serviços gRPC para o sistema de votação:

## voter.proto
- **Serviço**: VoterRegistrationService
- **Método**: IssueVotingCredential
- **Função**: Emite credenciais de voto para eleitores válidos

## voting.proto
- **Serviço**: VotingService
- **Métodos**:
  1. GetCandidates - Lista candidatos disponíveis
  2. Vote - Regista um voto com credencial
  3. GetResults - Obtém resultados eleitorais

## Endpoint do serviço
ken01.utad.pt:9091

