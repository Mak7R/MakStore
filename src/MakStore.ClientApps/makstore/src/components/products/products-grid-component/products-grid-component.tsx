'use client'

import React, {useEffect} from 'react';
import useAuthAxios from "@/hooks/use-auth-axios";
import {endpoints} from "@/config/endpoints";
import useAppQuery from "@/hooks/use-app-query";
import DefaultLoadingMessage from "@/components/ui/loading/messages/default-loading-message";
import SuccessButton from "@/components/ui/buttons/success-button";
import DangerButton from "@/components/ui/buttons/danger-button";
import InfoButton from "@/components/ui/buttons/info-button";
import PayButton from "@/components/ui/buttons/pay-button";
import HotButton from "@/components/ui/buttons/hot-button";
import WarningButton from "@/components/ui/buttons/warning-button";
import DefaultButton from "@/components/ui/buttons/default-button";
import SecondaryButton from "@/components/ui/buttons/secondary-button";
import DefaultCheckbox from "@/components/ui/inputs/checkboxes/default-checkbox";
import ProductCard from "@/components/products/product-card";

interface ProductsGridComponentProps {

}

export default function ProductsGridComponent(props: ProductsGridComponentProps) {
  const axios = useAuthAxios()

  const query = useAppQuery<{id: string, name: string, description?: string, price: number}[]>(endpoints.products.getAll())
  
  const createProduct = () => {
    axios.post(endpoints.products.create(), {name: "Ax", description: "some description", price: -100})
      .then(r => {
        console.log(r.data);
      })
      .catch(e => {
        console.error(e);
        console.log(e.response.data);
      })
  }
  
  useEffect(() => {
   
  }, []);
  
  return (
    <>
      <div className='w-full flex flex-col items-center justify-center mb-12 mt-8'>
        {
          query.isLoading ? <DefaultLoadingMessage />
            :
            <div className="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-3 lg:grid-cols-4 gap-4 w-full">
              {
                query.data && query.data?.map(item => (
                  <ProductCard
                    key={item.id}
                    name={item.name}
                    price={item.price}
                    description={item.description}
                  />
                ))
              }
            </div>
        }
      </div>

      <div className='flex gap-4'>
        <SuccessButton onClick={createProduct} style={{height: 60, width: 140, fontSize: 16}}>Create</SuccessButton>
        <InfoButton onClick={createProduct} enableIcon={true} style={{height: 60, width: 140, fontSize: 16}}>Create</InfoButton>
        <DangerButton onClick={createProduct} enableIcon={true} style={{height: 60, width: 140, fontSize: 16}}>Create</DangerButton>
        <HotButton onClick={createProduct} enableIcon={true} style={{height: 60, width: 140, fontSize: 16}}>Create</HotButton>
        <WarningButton onClick={createProduct} style={{height: 60, width: 140, fontSize: 16}}>Create</WarningButton>
        <PayButton onClick={createProduct} enableIcon={true} style={{height: 60, width: 140, fontSize: 16}}>Create</PayButton>
        <SecondaryButton onClick={createProduct} style={{height: 60, width: 140, fontSize: 16}}>Create</SecondaryButton>
        <DefaultButton onClick={createProduct} style={{height: 60, width: 140, fontSize: 16}}>Create</DefaultButton>
      </div>
      
      
    </>
  );
}