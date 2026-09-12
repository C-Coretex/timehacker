import type { CategoryDisplayModel, CategoryFormData } from '../../api/types';

export interface CategoryFormModalProps {
  open: boolean;
  onCancel: () => void;
  onSave: (data: CategoryFormData, id?: string) => void;
  initialData?: CategoryDisplayModel | null;
}
