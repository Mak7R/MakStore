import React, {ButtonHTMLAttributes} from 'react';
import styles from './index.module.css'
interface SuccessButtonProps extends ButtonHTMLAttributes<HTMLButtonElement> {
  children?: React.ReactNode
}

export default function SuccessButton(props: SuccessButtonProps) {
  return (
    <button className={styles.button} {...props}>
      <span>{props.children}</span>
    </button>
  );
}