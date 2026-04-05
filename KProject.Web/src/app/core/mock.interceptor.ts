import {HttpInterceptorFn, HttpResponse} from '@angular/common/http';
import {of} from 'rxjs';
import {delay} from 'rxjs/operators';

const randomDelay = () => Math.floor(Math.random() * 800) + 200;

const mockClients = [
    {id: 1, nome: 'Maria Silva'},
    {id: 2, nome: 'Joao Santos'},
    {id: 3, nome: 'Ana Oliveira'},
    {id: 4, nome: 'Carlos Pereira'},
    {id: 5, nome: 'Lucia Ferreira'},
    {id: 6, nome: 'Pedro Costa'},
    {id: 7, nome: 'Fernanda Lima'},
];

const mockSaleProducts = [
    {id: 1, nome: 'Camiseta Basica Branca', totalLotes: 3},
    {id: 2, nome: 'Calca Jeans Slim', totalLotes: 2},
    {id: 3, nome: 'Vestido Floral', totalLotes: 3},
    {id: 4, nome: 'Blusa Moletom Cinza', totalLotes: 1},
    {id: 5, nome: 'Saia Midi Preta', totalLotes: 2},
    {id: 6, nome: 'Jaqueta Couro Marrom', totalLotes: 1},
    {id: 7, nome: 'Shorts Linho Bege', totalLotes: 2},
    {id: 8, nome: 'Camisa Social Azul', totalLotes: 0},
];

const mockLotes: Record<number, object[]> = {
    1: [
        {id: 10, numero: 1, validade: '2027-06-15', quantidadeTotal: 10},
        {id: 11, numero: 2, validade: '2027-09-01', quantidadeTotal: 5},
        {id: 12, numero: 3, validade: '2026-12-31', quantidadeTotal: 3},
    ],
    2: [
        {id: 13, numero: 1, validade: '2028-01-10', quantidadeTotal: 8},
        {id: 14, numero: 2, validade: '2027-06-20', quantidadeTotal: 4},
    ],
    3: [
        {id: 15, numero: 1, validade: '2027-03-10', quantidadeTotal: 15},
        {id: 16, numero: 2, validade: '2027-07-22', quantidadeTotal: 7},
        {id: 17, numero: 3, validade: '2026-11-15', quantidadeTotal: 2},
    ],
    4: [
        {id: 18, numero: 1, validade: '2028-02-28', quantidadeTotal: 20},
    ],
    5: [
        {id: 4, numero: 201, validade: '2028-01-10', quantidadeTotal: 200},
        {id: 5, numero: 202, validade: '2027-06-20', quantidadeTotal: 75},
    ],
    6: [
        {id: 1, numero: 101, validade: '2027-03-15', quantidadeTotal: 48},
        {id: 2, numero: 102, validade: '2027-09-01', quantidadeTotal: 12},
        {id: 3, numero: 103, validade: '2026-12-31', quantidadeTotal: 5},
    ],
    7: [
        {id: 19, numero: 1, validade: '2027-05-01', quantidadeTotal: 6},
        {id: 20, numero: 2, validade: '2027-10-15', quantidadeTotal: 3},
    ],
};

const mockHistorico: Record<number, object[]> = {
    1: [
        {id: 1, tipo: 'Entrada', deltaQuantidade: 50, criadoEm: '2026-01-05T08:00:00Z', criadoPor: 'admin@wilasj.dev'},
        {id: 2, tipo: 'SaidaConsignacao', deltaQuantidade: -7, criadoEm: '2026-01-10T14:00:00Z', vendaId: 1042, criadoPor: 'admin@wilasj.dev'},
        {id: 3, tipo: 'RetornoConsignacao', deltaQuantidade: 3, criadoEm: '2026-01-15T09:00:00Z', vendaId: 1042, criadoPor: 'admin@wilasj.dev'},
        {id: 4, tipo: 'Entrada', deltaQuantidade: 30, criadoEm: '2026-01-20T10:00:00Z', criadoPor: 'joao@wilasj.dev'},
        {id: 5, tipo: 'SaidaConsignacao', deltaQuantidade: -10, criadoEm: '2026-01-25T11:00:00Z', vendaId: 1038, criadoPor: 'joao@wilasj.dev'},
        {id: 6, tipo: 'Ajuste', deltaQuantidade: -3, criadoEm: '2026-01-28T08:00:00Z', criadoPor: 'admin@wilasj.dev'},
        {id: 7, tipo: 'Entrada', deltaQuantidade: 20, criadoEm: '2026-02-02T10:00:00Z', criadoPor: 'admin@wilasj.dev'},
        {id: 8, tipo: 'Perda', deltaQuantidade: -5, criadoEm: '2026-02-05T16:00:00Z', criadoPor: 'joao@wilasj.dev'},
        {id: 9, tipo: 'SaidaConsignacao', deltaQuantidade: -15, criadoEm: '2026-02-10T10:00:00Z', vendaId: 1040, criadoPor: 'admin@wilasj.dev'},
        {id: 10, tipo: 'RetornoConsignacao', deltaQuantidade: 8, criadoEm: '2026-02-14T14:00:00Z', vendaId: 1040, criadoPor: 'admin@wilasj.dev'},
        {id: 11, tipo: 'Entrada', deltaQuantidade: 40, criadoEm: '2026-02-18T10:00:00Z', criadoPor: 'joao@wilasj.dev'},
        {id: 12, tipo: 'SaidaConsignacao', deltaQuantidade: -12, criadoEm: '2026-02-22T11:00:00Z', vendaId: 1037, criadoPor: 'joao@wilasj.dev'},
        {id: 13, tipo: 'Ajuste', deltaQuantidade: 2, criadoEm: '2026-02-25T09:00:00Z', criadoPor: 'admin@wilasj.dev'},
        {id: 14, tipo: 'Entrada', deltaQuantidade: 25, criadoEm: '2026-03-01T10:00:00Z', criadoPor: 'admin@wilasj.dev'},
        {id: 15, tipo: 'Perda', deltaQuantidade: -4, criadoEm: '2026-03-03T15:00:00Z', criadoPor: 'joao@wilasj.dev'},
        {id: 16, tipo: 'SaidaConsignacao', deltaQuantidade: -8, criadoEm: '2026-03-05T11:00:00Z', vendaId: 1036, criadoPor: 'admin@wilasj.dev'},
        {id: 17, tipo: 'RetornoConsignacao', deltaQuantidade: 5, criadoEm: '2026-03-07T09:00:00Z', vendaId: 1036, criadoPor: 'admin@wilasj.dev'},
        {id: 18, tipo: 'Entrada', deltaQuantidade: 35, criadoEm: '2026-03-09T10:00:00Z', criadoPor: 'joao@wilasj.dev'},
        {id: 19, tipo: 'SaidaConsignacao', deltaQuantidade: -20, criadoEm: '2026-03-11T14:00:00Z', vendaId: 1034, criadoPor: 'joao@wilasj.dev'},
        {id: 20, tipo: 'Ajuste', deltaQuantidade: -1, criadoEm: '2026-03-12T08:00:00Z', criadoPor: 'admin@wilasj.dev'},
        {id: 21, tipo: 'Entrada', deltaQuantidade: 60, criadoEm: '2026-03-14T10:00:00Z', criadoPor: 'admin@wilasj.dev'},
        {id: 22, tipo: 'Perda', deltaQuantidade: -9, criadoEm: '2026-03-15T16:00:00Z', criadoPor: 'joao@wilasj.dev'},
        {id: 23, tipo: 'SaidaConsignacao', deltaQuantidade: -18, criadoEm: '2026-03-17T11:00:00Z', vendaId: 1033, criadoPor: 'admin@wilasj.dev'},
        {id: 24, tipo: 'RetornoConsignacao', deltaQuantidade: 10, criadoEm: '2026-03-19T09:00:00Z', vendaId: 1033, criadoPor: 'admin@wilasj.dev'},
        {id: 25, tipo: 'Entrada', deltaQuantidade: 45, criadoEm: '2026-03-21T10:00:00Z', criadoPor: 'joao@wilasj.dev'},
    ],
    2: [
        {id: 13, tipo: 'Entrada', deltaQuantidade: 12, criadoEm: '2026-03-01T10:00:00Z', criadoPor: 'admin@wilasj.dev'},
    ],
    3: [
        {id: 14, tipo: 'Entrada', deltaQuantidade: 20, criadoEm: '2026-02-15T09:00:00Z', criadoPor: 'admin@wilasj.dev'},
        {id: 15, tipo: 'Perda', deltaQuantidade: -15, criadoEm: '2026-03-01T11:00:00Z', criadoPor: 'joao@wilasj.dev'},
    ],
};

const mockSales = [
    {id: 1042, clienteNome: 'Maria Silva', criadaEm: '2026-02-27T10:00:00Z', status: 'Aberta', totalItens: 8},
    {id: 1041, clienteNome: 'João Santos', criadaEm: '2026-02-26T09:00:00Z', status: 'Fechada', totalItens: 5},
    {id: 1040, clienteNome: 'Ana Oliveira', criadaEm: '2026-02-25T14:00:00Z', status: 'Aberta', totalItens: 12},
    {id: 1039, clienteNome: 'Carlos Pereira', criadaEm: '2026-02-24T11:00:00Z', status: 'Cancelada', totalItens: 3},
    {id: 1038, clienteNome: 'Lucia Ferreira', criadaEm: '2026-02-23T08:00:00Z', status: 'Fechada', totalItens: 10},
    {id: 1037, clienteNome: 'Pedro Costa', criadaEm: '2026-02-22T15:00:00Z', status: 'Aberta', totalItens: 4},
    {id: 1036, clienteNome: 'Fernanda Lima', criadaEm: '2026-02-21T10:00:00Z', status: 'Fechada', totalItens: 6},
    {id: 1035, clienteNome: 'Bruno Alves', criadaEm: '2026-02-20T13:00:00Z', status: 'Cancelada', totalItens: 2},
    {id: 1034, clienteNome: 'Sofia Rocha', criadaEm: '2026-02-19T09:00:00Z', status: 'Aberta', totalItens: 9},
    {id: 1033, clienteNome: 'Diego Nunes', criadaEm: '2026-02-18T16:00:00Z', status: 'Fechada', totalItens: 7},
    {id: 1032, clienteNome: 'Laura Pinto', criadaEm: '2026-02-17T11:00:00Z', status: 'Aberta', totalItens: 14},
    {id: 1031, clienteNome: 'Ricardo Souza', criadaEm: '2026-02-16T08:00:00Z', status: 'Fechada', totalItens: 1},
];

export const mockInterceptor: HttpInterceptorFn = (req, next) => {
    if (req.url === '/api/clientes' && req.method === 'POST') {
        const body = req.body as {nome: string};
        const newClient = {id: mockClients.length + 1, nome: body.nome};
        mockClients.push(newClient);
        return of(new HttpResponse({status: 201, body: {id: newClient.id}})).pipe(delay(randomDelay()));
    }

    if (req.url === '/api/clientes' && req.method === 'GET') {
        const busca = req.params.get('busca')?.toLowerCase() ?? '';
        const pagina = Number(req.params.get('pagina') ?? 1);
        const tamanhoPagina = Number(req.params.get('tamanhoPagina') ?? 10);
        const filtered = busca
            ? mockClients.filter(c => c.nome.toLowerCase().includes(busca))
            : mockClients;
        const start = (pagina - 1) * tamanhoPagina;
        const items = filtered.slice(start, start + tamanhoPagina);
        return of(new HttpResponse({status: 200, body: {items, total: filtered.length}})).pipe(delay(randomDelay()));
    }

    if (req.url === '/api/produtos' && req.method === 'GET' && req.params.has('busca')) {
        const busca = req.params.get('busca')?.toLowerCase() ?? '';
        const items = busca
            ? mockSaleProducts.filter(p => p.nome.toLowerCase().includes(busca))
            : mockSaleProducts;
        return of(new HttpResponse({status: 200, body: {items, total: items.length}})).pipe(delay(randomDelay()));
    }

    if (req.url === '/api/vendas' && req.method === 'POST') {
        return of(new HttpResponse({status: 201, body: {id: Math.floor(Math.random() * 9000) + 1000}}))
            .pipe(delay(800));
    }

    const produtoLotesMatch = req.url.match(/^\/api\/produtos\/(\d+)\/lotes$/);
    if (produtoLotesMatch) {
        const productId = Number(produtoLotesMatch[1]);
        const lotes = mockLotes[productId] ?? [];
        return of(new HttpResponse({status: 200, body: lotes})).pipe(delay(randomDelay()));
    }

    const loteHistoricoMatch = req.url.match(/^\/api\/lotes\/(\d+)\/historico$/);
    if (loteHistoricoMatch && req.method === 'GET') {
        const loteId = Number(loteHistoricoMatch[1]);
        const all = mockHistorico[loteId] ?? [];
        const pagina = Number(req.params.get('pagina') ?? 1);
        const tamanhoPagina = Number(req.params.get('tamanhoPagina') ?? 20);
        const start = (pagina - 1) * tamanhoPagina;
        const items = all.slice(start, start + tamanhoPagina);
        const hasMore = start + tamanhoPagina < all.length;
        return of(new HttpResponse({status: 200, body: {items, hasMore}})).pipe(delay(randomDelay()));
    }

    const vendaDetalheMatch = req.url.match(/^\/api\/vendas\/(\d+)$/);
    if (vendaDetalheMatch && req.method === 'GET') {
        const id = Number(vendaDetalheMatch[1]);
        const body = id === 1042
            ? {
                id: 1042,
                status: 'Aberta',
                criadaEm: '2026-02-27T10:00:00Z',
                modificadaEm: '2026-02-27T10:00:00Z',
                criadaPor: 'Admin',
                clienteNome: 'Maria Silva',
                itens: [
                    {id: 1, produtoNome: 'Camiseta Basica Branca P', loteNumero: 1, pacienteNome: 'Paciente A', quantidadeConsignada: 10, vendido: 8, devolvido: 1},
                    {id: 2, produtoNome: 'Calca Jeans Slim M',       loteNumero: 1, pacienteNome: 'Paciente B', quantidadeConsignada: 5,  vendido: 4, devolvido: 0},
                    {id: 3, produtoNome: 'Vestido Floral G',          loteNumero: 2, pacienteNome: 'Paciente C', quantidadeConsignada: 8,  vendido: 5, devolvido: 2},
                    {id: 4, produtoNome: 'Blusa Moletom Cinza GG',   loteNumero: 1, pacienteNome: 'Paciente D', quantidadeConsignada: 6,  vendido: 3, devolvido: 1},
                    {id: 5, produtoNome: 'Saia Midi Preta M',         loteNumero: 1, pacienteNome: 'Paciente E', quantidadeConsignada: 4,  vendido: 2, devolvido: 0},
                ],
              }
            : {
                id,
                status: mockSales.find(s => s.id === id)?.status ?? 'Aberta',
                criadaEm: '2026-01-01T10:00:00Z',
                modificadaEm: '2026-01-01T10:00:00Z',
                criadaPor: 'Admin',
                clienteNome: mockSales.find(s => s.id === id)?.clienteNome ?? 'Cliente Generico',
                itens: [
                    {id: 99, produtoNome: 'Produto Generico', loteNumero: 1, pacienteNome: 'Paciente X', quantidadeConsignada: 5, vendido: 3, devolvido: 1},
                ],
              };
        return of(new HttpResponse({status: 200, body})).pipe(delay(randomDelay()));
    }

    if (vendaDetalheMatch && req.method === 'PATCH') {
        return of(new HttpResponse({status: 204})).pipe(delay(randomDelay()));
    }

    const vendaActionMatch = req.url.match(/^\/api\/vendas\/(\d+)\/(close|cancel)$/);
    if (vendaActionMatch && req.method === 'POST') {
        return of(new HttpResponse({status: 204})).pipe(delay(randomDelay()));
    }

    if (req.url === '/api/vendas' && req.method === 'GET') {
        const pagina = Number(req.params.get('pagina') ?? 1);
        const tamanhoPagina = Number(req.params.get('tamanhoPagina') ?? 10);
        const busca = req.params.get('busca')?.toLowerCase() ?? '';
        const filtered = busca
            ? mockSales.filter(s => s.clienteNome.toLowerCase().includes(busca))
            : mockSales;
        const start = (pagina - 1) * tamanhoPagina;
        const items = filtered.slice(start, start + tamanhoPagina);
        return of(new HttpResponse({status: 200, body: {items, total: filtered.length}})).pipe(delay(randomDelay()));
    }

    return next(req);
};
