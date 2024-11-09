import React, {ButtonHTMLAttributes} from 'react';
import styles from './index.module.css'

interface SecondaryButtonProps extends ButtonHTMLAttributes<HTMLButtonElement> {
  children?: React.ReactNode
}

export default function SecondaryButton(props: SecondaryButtonProps) {
  return (
    <button className={styles.button} {...props}>
      <span>{props.children}</span>
    </button>
  );
}