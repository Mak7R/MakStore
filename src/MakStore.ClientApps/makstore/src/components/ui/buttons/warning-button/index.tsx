import React, {ButtonHTMLAttributes} from 'react';
import styles from './index.module.css'
interface WarningButtonProps extends ButtonHTMLAttributes<HTMLButtonElement> {
  children?: React.ReactNode;
}

export default function WarningButton(props: WarningButtonProps) {
  return (
    <button {...props} className={styles.button}><span>{props.children}</span></button>
  );
}