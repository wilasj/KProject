export type StatusVenda = 'Aberta' | 'Fechada' | 'Cancelada';

export interface Sale {
  id: number;
  clienteNome: string;
  criadaEm: string;
  status: StatusVenda;
  totalItens: number;
}

export interface SalesResponse {
    items: Sale[];
    total: number;
}

export interface SaleItemDetail {
    id: number;
    produtoNome: string;
    loteNumero: number;
    pacienteNome: string;
    quantidadeConsignada: number;
    vendido: number;
    devolvido: number;
}

export interface SaleDetail {
    id: number;
    status: StatusVenda;
    criadaEm: string;
    criadaPor: string;
    modificadaEm: string;
    clienteNome: string;
    itens: SaleItemDetail[];
}
