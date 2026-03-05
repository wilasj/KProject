export interface Lote {
  id: number;
  numero: number;
  validade: string;
  quantidadeTotal: number;
}

export interface StockMovement {
  id: number;
  tipo: string;
  deltaQuantidade: number;
  criadoEm: string;
}

export interface LoteDetail extends Lote {
  historico: StockMovement[];
}
