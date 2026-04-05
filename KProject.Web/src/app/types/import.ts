export type ChangeLineType = 'StockEntry' | 'NewLot' | 'NewProduct' | 'Ambiguous';

export interface ChangeLine {
  id: string;
  type: ChangeLineType;
  productName: string;
  referencia: string;
  loteNumero: number;
  validade: string;
  quantidade: number;
  candidates?: { id: string; nome: string; referencia: string }[];
  resolution?: { type: 'link'; produtoId: string } | { type: 'new' };
}

export interface ImportTask {
  id: string;
  fileName: string;
  status: 'pending' | 'processing' | 'review' | 'done' | 'error';
  createdAt: string;
  errorMessage?: string;
  changeLines?: ChangeLine[];
}
