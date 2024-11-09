import React from 'react';
import ProductsGridComponent from "@/components/products/products-grid-component/products-grid-component";

interface ProductsPageProps {

}

export default function ProductsPage(props: ProductsPageProps) {
  return (
    <>
      <div className='p-8'>
        <h2 className='text-center text-3xl font-bold'>Products</h2>
        <ProductsGridComponent />
      </div>
    </>
  );
}