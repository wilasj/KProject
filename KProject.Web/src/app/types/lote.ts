export interface Lote {
  id: number;
  numero: number;
  validade: string;
  quantidadeTotal: number;
}

export type TipoHistorico =
  | 'Entrada'
  | 'SaidaConsignacao'
  | 'RetornoConsignacao'
  | 'Ajuste'
  | 'Perda';

export interface StockMovement {
  id: number;
  tipo: TipoHistorico;
  deltaQuantidade: number;
  criadoEm: string;
}

export interface LoteDetail extends Lote {
  historico: StockMovement[];
}
