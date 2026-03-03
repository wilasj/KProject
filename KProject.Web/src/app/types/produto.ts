export interface Product {
  id: number;
  nome: string;
  referencia: string;
  descricao: string;
  codigoAnvisa: string;
  criadoEm: string;
}

export interface ProductsResponse {
  items: Product[];
  total: number;
}
