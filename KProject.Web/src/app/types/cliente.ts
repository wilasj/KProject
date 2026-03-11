export interface Cliente {
  id: number;
  nome: string;
}

export interface ClientesResponse {
  items: Cliente[];
  total: number;
}
