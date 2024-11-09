const productsServiceAddress = process.env.NEXT_PUBLIC_PRODUCTS_SERVICE_URL;

export const endpoints = {
  products: {
    getAll: () => `${productsServiceAddress}/api/v1/products`,
    getById: (id: string) => `${productsServiceAddress}/api/v1/products/${id}`,
    create: () => `${productsServiceAddress}/api/v1/products`,
    update: (id: string) => `${productsServiceAddress}/api/v1/products/${id}`,
    delete: (id: string) => `${productsServiceAddress}/api/v1/products/${id}`,
  }
}