import { PaginatedResponse } from './paginated-response';

export interface Product {
  id: number;
  nome: string;
  referencia: string;
  descricao: string;
  codigoAnvisa: string;
  criadoEm: string;
}

export type ProductsResponse = PaginatedResponse<Product>;
