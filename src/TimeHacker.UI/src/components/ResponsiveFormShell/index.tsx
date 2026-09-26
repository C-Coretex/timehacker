import type { FC, ReactNode } from 'react';
import { Button, Modal } from 'antd';
import { PlusCircleFilled, SaveOutlined } from '@ant-design/icons';
import './styles.css';

interface ResponsiveFormShellProps {
  open: boolean;
  onCancel: () => void;
  title: ReactNode;
  submitLabel: string;
  onSubmit: () => void;
  /** Editing swaps the ⊕ for a save icon. */
  isEdit?: boolean;
  width?: number;
  children: ReactNode;
}

/**
 * Frame shared by the create/edit forms: a centred dialog on desktop, a full-screen sheet on phones, and the
 * design's full-width magenta submit bar.
 */
export const ResponsiveFormShell: FC<ResponsiveFormShellProps> = ({
  open,
  onCancel,
  title,
  submitLabel,
  onSubmit,
  isEdit = false,
  width = 820,
  children,
}) => (
  <Modal
    open={open}
    onCancel={onCancel}
    title={title}
    width={width}
    rootClassName="th-form-shell"
    forceRender
    destroyOnHidden
    footer={
      <Button
        type="primary"
        block
        size="large"
        className="th-form-shell__submit"
        icon={isEdit ? <SaveOutlined /> : <PlusCircleFilled />}
        iconPlacement="end"
        onClick={onSubmit}
      >
        {submitLabel}
      </Button>
    }
  >
    {children}
  </Modal>
);
