import { PaginatedResponse } from './paginated-response';

export interface Cliente {
  id: number;
  nome: string;
}

export type ClientesResponse = PaginatedResponse<Cliente>;
