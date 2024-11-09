import React, {ButtonHTMLAttributes} from 'react';
import SuccessButton from "@/components/ui/buttons/success-button";

interface DefaultButtonProps extends ButtonHTMLAttributes<HTMLButtonElement> {
  children?: React.ReactNode;
}

export default function DefaultButton(props: DefaultButtonProps) {
  return <SuccessButton {...props}/>
}