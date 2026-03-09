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
